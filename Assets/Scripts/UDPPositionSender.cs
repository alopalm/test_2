using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using UnityEngine.XR;

public class UDPPositionSender : MonoBehaviour
{
    [Header("Configuración de Red")]
    public string ipServidor = "192.168.1.101"; // IP de tu Niryo
    public int puerto = 5005;
    
    [Range(10, 60)]
    public int frecuenciaEnvio = 30; // 30Hz

    [Header("Target Tracking")]
    public Transform targetPosicion;

    [Header("Configuración del Mando (Oculus)")]
    public XRNode tipoMando = XRNode.RightHand; 

    [Header("Límites Reales del Niryo Ned 2 (ROS)")]
    public float minX_Robot = 0.12f;  public float maxX_Robot = 0.32f; 
    public float minY_Robot = -0.15f; public float maxY_Robot = 0.15f; 
    public float minZ_Robot = 0.16f;  public float maxZ_Robot = 0.35f; 

    [Header("Rangos del Cubo de Unity")]
    public float minX_Uni = -0.40f;   public float maxX_Uni = 0.40f;
    public float minY_Uni = -0.30f;   public float maxY_Uni = 0.40f;
    public float minZ_Uni = 0.25f;    public float maxZ_Uni = 0.80f;

    private UdpClient clienteUdp;
    private IPEndPoint puntoFinal;
    private float intervaloEnvio;
    private float tiempoUltimoEnvio = 0f;
    private static readonly CultureInfo INV = CultureInfo.InvariantCulture;

    // Variables internas para el "Efecto Ratón" (Clutching)
    private bool estabaPulsadoElBotonoAnteriormente = false;
    private Vector3 posicionManoAlEmbragar = Vector3.zero;
    private Vector3 posicionRobotAlEmbragar = new Vector3(0.146f, 0.000f, 0.215f); // Arranca en el Home de tu foto
    
    // Coordenadas consolidadas que viajan al robot
    private float sendX, sendY, sendZ;

    void Start()
    {
        if (targetPosicion == null) targetPosicion = transform;
        intervaloEnvio = 1f / frecuenciaEnvio;

        // Inicializamos los valores de envío en el Home real del robot
        sendX = posicionRobotAlEmbragar.x;
        sendY = posicionRobotAlEmbragar.y;
        sendZ = posicionRobotAlEmbragar.z;

        try
        {
            clienteUdp = new UdpClient();
            puntoFinal = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);
            Debug.Log($"[ClutchUDP] Inicializado en puerto {puerto}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ClutchUDP] Error: {e.Message}");
        }
    }

    void FixedUpdate()
    {
        if (Time.time - tiempoUltimoEnvio >= intervaloEnvio)
        {
            ProcesarTeleoperacionConEmbrague();
            tiempoUltimoEnvio = Time.time;
        }
    }

    void ProcesarTeleoperacionConEmbrague()
    {
        if (clienteUdp == null || targetPosicion == null) return;

        // 1. Detectar el mando de Oculus y leer el Gatillo del Índice
        var device = InputDevices.GetDeviceAtXRNode(tipoMando);
        float valorGatillo = 0f;
        device.TryGetFeatureValue(CommonUsages.trigger, out valorGatillo);

        // Consideramos "pulsado" si hundes el gatillo más del 30%
        bool botonPulsado = valorGatillo > 0.3f; 

        Vector3 posManoActual = targetPosicion.localPosition;

        if (botonPulsado)
        {
            // ── MOMENTO JUSTO DE APRETAR EL BOTÓN (Flanco de subida) ──
            if (!estabaPulsadoElBotonoAnteriormente)
            {
       
       
                posicionManoAlEmbragar = posManoActual;
                posicionRobotAlEmbragar = new Vector3(sendX, sendY, sendZ);
                estabaPulsadoElBotonoAnteriormente = true;
                Debug.Log("[EMBRAGUE] Conectado. Transmitiendo movimiento...");
            }

            // ── MIENTRAS MANTIENES PULSADO ──
            // Calculamos cuánto te has desplazado respecto al punto donde hiciste el "clic"
            float deltaX_Unity = posManoActual.z - posicionManoAlEmbragar.z;
            float deltaY_Unity = posManoActual.x - posicionManoAlEmbragar.x;
            float deltaZ_Unity = posManoActual.y - posicionManoAlEmbragar.y;

            // Mapeamos los desplazamientos relativos de forma directa y proporcional
            // Multiplicamos por un factor de escala si quieres que el robot se estire más o menos que tu brazo
            float moverX_ROS = deltaX_Unity * 0.8f; 
            float moverY_ROS = -deltaY_Unity * 0.8f; // Inversión de espejo intuitiva
            float moverZ_ROS = deltaZ_Unity * 0.8f;

            // La nueva coordenada es la posición que tenía el robot al embragar + el desplazamiento actual
            sendX = posicionRobotAlEmbragar.x + moverX_ROS;
            sendY = posicionRobotAlEmbragar.y + moverY_ROS;
            sendZ = posicionRobotAlEmbragar.z + moverZ_ROS;

            // Acotamos estrictamente con tus límites de NiryoStudio
            sendX = Mathf.Clamp(sendX, 0.10f, 0.35f);
            sendY = Mathf.Clamp(sendY, -0.18f, 0.18f);
            sendZ = Mathf.Clamp(sendZ, 0.15f, 0.38f);
        }
        else
        {
            // ── SI SUELTAS EL BOTÓN ──
            if (estabaPulsadoElBotonoAnteriormente)
            {
                estabaPulsadoElBotonoAnteriormente = false;
                Debug.Log("[EMBRAGUE] Soltado. Posición del robot congelada.");
            }
            // Al estar suelto, seguimos enviando las últimas coordenadas fijas (sendX, sendY, sendZ)
            // Esto le dice al robot: "Quédate exactamente donde estás, error = 0.0"
        }

        // 2. Empaquetar y enviar el paquete UDP (Rotación capada a 0.0)
        try
        {
            string mensaje = string.Format(INV, "{0:F4},{1:F4},{2:F4},0.0,0.0,0.0,1.0", sendX, sendY, sendZ);
            byte[] datos = Encoding.UTF8.GetBytes(mensaje);
            clienteUdp.BeginSend(datos, datos.Length, puntoFinal, null, null);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[UDP] Error: " + e.Message);
        }
    }
}
