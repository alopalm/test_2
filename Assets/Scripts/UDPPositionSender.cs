using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class UDPPositionSender : MonoBehaviour
{
    [Header("Configuración de Red")]
    public string ipServidor = "192.168.1.102"; // IP del ordenador con Python/Ubuntu
    public int puerto = 5005;

    [Header("Configuración de Envío")]
    [Tooltip("Veces por segundo que se envían datos")]
    public float frecuenciaEnvio = 20f; 
    
    private UdpClient clienteUdp;
    private IPEndPoint puntoFinal;
    private float proximoEnvio = 0f;

    void Start()
    {
        // Inicializamos el socket UDP
        clienteUdp = new UdpClient();
        puntoFinal = new IPEndPoint(IPAddress.Parse(ipServidor), puerto);
        
        Debug.Log($"NSOCKET_START_SOCKET");
    }

    void Update()
    {
        // Controlamos la frecuencia para no saturar la red ni al robot
        if (Time.time >= proximoEnvio)
        {
            EnviarPosicion();
            proximoEnvio = Time.time + (1f / frecuenciaEnvio);
        }
    }

void EnviarPosicion()
{
    try
    {
        Vector3 pos = transform.position;
        string mensaje = string.Format(System.Globalization.CultureInfo.InvariantCulture, 
            "{0:F4},{1:F4},{2:F4}", pos.x, pos.y, pos.z);

        byte[] datos = Encoding.UTF8.GetBytes(mensaje);
        clienteUdp.Send(datos, datos.Length, puntoFinal);

        // AÑADE ESTA LÍNEA PARA DEBUG:
        Debug.Log($"NSOCKET_SENDING: {mensaje} to {ipServidor}");
    }
    catch (Exception e)
    {
        Debug.LogError("NSOCKET_ERROR: " + e.Message);
    }
}

    // Es importante cerrar el socket al detener Unity
    void OnApplicationQuit()
    {
        if (clienteUdp != null)
            clienteUdp.Close();
    }
}
