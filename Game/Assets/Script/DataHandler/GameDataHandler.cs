using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UnityEngine;

public class GameDataHandler : MonoBehaviour, IConnectDataHandler
{
    // Start is called before the first frame update
    void Start()
    {
        WebSocketManager.Instance.AddSocket("ws://192.168.50.51:5000/", this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConnected(string id, ClientWebSocket webSocket)
    {

    }

    public void OnDisconnect(string id)
    {

    }

    public void Receive(string id, ClientWebSocket webSocket, string requestContent)
    {

    }
}
