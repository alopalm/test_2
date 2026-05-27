using UnityEngine;

public class NetworkManager : PersistentSingleton<NetworkManager>
{
    [Header("UDP")]
    public string pythonIP = "127.0.0.1";
    public int pythonPort = 5055;

    public bool IsConnected => false;
}
