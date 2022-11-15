using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class WebSocketMiddleWare
    {
        private readonly IConnectDataHandler _dataHandler;
        private readonly WebSocketManager _webSocketManager;
        private readonly RequestDelegate _next;
        private readonly ILogger<WebSocketMiddleWare> _logger;

        public WebSocketMiddleWare(IConnectDataHandler dataHandler, WebSocketManager webSocketManager, RequestDelegate next, ILogger<WebSocketMiddleWare> logger)
        {
            _dataHandler = dataHandler;
            _webSocketManager = webSocketManager;
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                var id = _webSocketManager.AddSocket(webSocket);
                await ReceiveAsync(id, webSocket);
            }
            else
            { 
                await _next(httpContext);
            }
        }

        private async Task ReceiveAsync(string id, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocketManager.RemoveSocket(id, result.CloseStatusDescription);
                        return;
                    }
                    _dataHandler.Receive(id, webSocket, Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                catch
                {
                    await _webSocketManager.RemoveSocket(id, "game client: connection exception close.");
                }
            }
        }
    }
}
