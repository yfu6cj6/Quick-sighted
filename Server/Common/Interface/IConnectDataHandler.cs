using System.Collections.Generic;
using System.Net.WebSockets;

namespace Common
{
    public interface IConnectDataHandler
    {
        void Receive(string id, WebSocket webSocket, string requestContent);
    }
}
