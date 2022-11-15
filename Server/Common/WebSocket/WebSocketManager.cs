using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class WebSocketManager
    {
        private ConcurrentDictionary<string, WebSocket> _webSockets = new ConcurrentDictionary<string, WebSocket>();

        public string AddSocket(WebSocket webSocket)
        {
            string id = CreateConnectionId();
            while (!_webSockets.TryAdd(id, webSocket))
            {
                id = CreateConnectionId();
            }

            return id;
        }

        public async Task RemoveSocket(string id, string closeStatusDescription)
        {
            WebSocket webSocket;
            _webSockets.TryRemove(id, out webSocket);

            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                        statusDescription: closeStatusDescription,
                        cancellationToken: CancellationToken.None);
            }
        }

        public async Task SendMessageAsync(WebSocket webSocket, string message)
        {
            if (webSocket.State != WebSocketState.Open)
                return;

            byte[] buffer = Encoding.UTF8.GetBytes(message);

            await webSocket.SendAsync(buffer: new ArraySegment<byte>(array: buffer, offset: 0, count: buffer.Length),
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var webSocket in _webSockets)
            {
                if (webSocket.Value.State == WebSocketState.Open)
                    await SendMessageAsync(webSocket.Value, message);
            }
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
