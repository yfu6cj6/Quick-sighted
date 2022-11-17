using System.Collections.Generic;
using System.Net.WebSockets;

namespace Common
{
    public interface IConnectDataHandler
    {
        void OnConnected(string id, WebSocket webSocket);

        void OnDisconnect(string id);

        void Receive(string id, WebSocket webSocket, string requestContent);
    }
}
