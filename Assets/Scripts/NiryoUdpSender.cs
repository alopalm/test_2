using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

[System.Serializable]
public class NedPoseMessage
{
    public string cmd;
    public float x;
    public float y;
    public float z;
    public float roll;
    public float pitch;
    public float yaw;
}

public class NiryoUdpSender : MonoBehaviour
{
    public string pythonIP = "192.168.1.102";
    public int pythonPort = 5055;

    private UdpClient udpClient;

    void Start()
    {
        udpClient = new UdpClient();
        Debug.Log("NIRYO_TEST_PY_START");

        Invoke(nameof(SendTest), 3f);
    }

    public void SendTest()
{
    Debug.Log("NIRYO_TEST_PY_SEND");

    NedPoseMessage msg = new NedPoseMessage
    {
        cmd = "move_pose",
        x = 0.3f,
        y = 0.1f,
        z = 0.2f,
        roll = 0.0f,
        pitch = 0.5f,
        yaw = 0.0f
    };

    string json = JsonUtility.ToJson(msg);
    Debug.Log("NIRYO_TEST_JSON: " + json);

    byte[] data = Encoding.UTF8.GetBytes(json);
    udpClient.Send(data, data.Length, pythonIP, pythonPort);

    Debug.Log("NIRYO_TEST_UDP_SEND " + pythonIP + ":" + pythonPort);
}
    void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}
