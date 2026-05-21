using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Globalization;

public class UDPPositionSender : MonoBehaviour
{
    [Header("Configuración de Red")]
    public string ipServidor = "192.168.1.102";
    public int puerto = 5005;
    
    [Tooltip("Frecuencia de envío de paquetes de red a ROS.")]
    [Range(10, 90)]
    public int frecuenciaEnvio = 50;

    [Header("Target Tracking")]
    [Tooltip("Arrastra aquí tu objeto 'mirroredHandTarget' (la bola).")]
    public Transform targetPosicion;

    [Header("Límites Reales del Niryo Ned 2 (ROS)")]
    public float minX_Robot = 0.12f;  public float maxX_Robot = 0.32f; // Adelante/atrás en ROS (Basado en Home)
    public float minY_Robot = -0.15f; public float maxY_Robot = 0.15f; // Izquierda/derecha en ROS
    public float minZ_Robot = 0.16f;  public float maxZ_Robot = 0.35f; // Arriba/abajo en ROS (Basado en Home)

    [Header("Rangos del Cubo de Unity (Sincronizados con MirrorController)")]
    public float minX_Uni = -0.40f;   public float maxX_Uni = 0.40f;
    public float minY_Uni = -0.30f;   public float maxY_Uni = 0.40f;
    public float minZ_Uni = 0.25f;    public float maxZ_Uni = 0.80f;

    private UdpClient clienteUdp;
    private IPEndPoint puntoFinal;
    
    private float intervaloEnvio;
    private float tiempoUltimoEnvio = 0f;

    private static readonly CultureInfo INV = CultureInfo.InvariantCulture;

    void Start()
    {
        if (targetPosicion == null)
            targetPosicion = transform;

        intervaloEnvio = 1f / frecuenciaEnvio;

        try
        {
            clienteUdp = new UdpClient();
            puntoFinal = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);
            Debug.Log($"[UDPSender] Conectado a la red UDP del Niryo -> {ipServidor}:{puerto}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UDPSender] Error al abrir socket UDP: {e.Message}");
        }
    }

    void FixedUpdate()
    {
        if (Time.time - tiempoUltimoEnvio >= intervaloEnvio)
        {
            EnviarDatosTeleoperacion();
            tiempoUltimoEnvio = Time.time;
        }
    }

    void EnviarDatosTeleoperacion()
    {
        if (clienteUdp == null || targetPosicion == null) return;

        try
        {
            Vector3 posUnity = targetPosicion.localPosition;
            Quaternion rotUnity = targetPosicion.localRotation;

            // FIX CRÍTICO: Ahora usamos las variables globales del Inspector de Unity
            float robotX = MapRange(posUnity.z, minZ_Uni, maxZ_Uni, minX_Robot, maxX_Robot);
            float robotY = MapRange(posUnity.x, minX_Uni, maxX_Uni, minY_Robot, maxY_Robot);
            float robotZ = MapRange(posUnity.y, minY_Uni, maxY_Uni, minZ_Robot, maxZ_Robot);

            // Remapeo de ejes de rotación para ROS
            Vector3 euler = rotUnity.eulerAngles;
            float rosRoll  = euler.z;  
            float rosPitch = euler.x;  
            float rosYaw   = euler.y;  
            Quaternion rotROS = Quaternion.Euler(rosPitch, rosYaw, rosRoll);

            string mensaje = string.Format(INV,
                "{0:F4},{1:F4},{2:F4},{3:F4},{4:F4},{5:F4},{6:F4}", 
                robotX, robotY, robotZ, 
                rotROS.x, rotROS.y, rotROS.z, rotROS.w
            );

            byte[] datos = Encoding.UTF8.GetBytes(mensaje);
            clienteUdp.BeginSend(datos, datos.Length, puntoFinal, null, null);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[UDPSender] Error de transmisión: " + e.Message);
        }
    }

    private float MapRange(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return fromTarget + (value - fromSource) * (toTarget - fromTarget) / (toSource - fromSource);
    }

    void OnApplicationQuit()
    {
        if (clienteUdp != null) clienteUdp.Close();
    }
}
