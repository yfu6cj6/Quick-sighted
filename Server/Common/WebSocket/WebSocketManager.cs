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
        private readonly ConcurrentDictionary<string, WebSocket> _webSockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task AddSocket(WebSocket webSocket, IConnectDataHandler dataHandler)
        {
            string id = CreateConnectionId();
            while (_webSockets.ContainsKey(id))
            {
                id = CreateConnectionId();
            }
            _webSockets.TryAdd(id, webSocket);

            dataHandler.OnConnected(id, webSocket);

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var buffer = new byte[1024 * 4];
                    var result = await webSocket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await RemoveSocket(id, result.CloseStatusDescription, dataHandler);
                        return;
                    }

                    dataHandler.Receive(id, webSocket, Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                catch
                {
                    await RemoveSocket(id, "game client: connection exception close.", dataHandler);
                }
            }
        }

        private async Task RemoveSocket(string id, string closeStatusDescription, IConnectDataHandler dataHandler)
        {
            _webSockets.TryRemove(id, out WebSocket webSocket);

            if (webSocket != null)
            {
                if (webSocket.State == WebSocketState.Open)
                { 
                    await webSocket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                            statusDescription: closeStatusDescription,
                            cancellationToken: CancellationToken.None);
                }
                webSocket.Abort();
                webSocket.Dispose();
            }
            dataHandler.OnDisconnect(id);
        }

        public async void SendMessageAsync(WebSocket webSocket, string message)
        {
            if (webSocket.State != WebSocketState.Open)
                return;

            byte[] buffer = Encoding.UTF8.GetBytes(message);

            await webSocket.SendAsync(buffer: new ArraySegment<byte>(array: buffer, offset: 0, count: buffer.Length),
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None);
        }

        public void SendMessageToAllAsync(string message)
        {
            foreach (var webSocket in _webSockets)
            {
                if (webSocket.Value.State == WebSocketState.Open)
                    SendMessageAsync(webSocket.Value, message);
            }
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
