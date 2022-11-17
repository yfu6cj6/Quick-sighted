using System.Net.WebSockets;

public interface IConnectDataHandler
{
    void OnConnected(string id, ClientWebSocket webSocket);

    void OnDisconnect(string id);

    void Receive(string id, ClientWebSocket webSocket, string requestContent);
}
