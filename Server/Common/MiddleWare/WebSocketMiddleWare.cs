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
                await _webSocketManager.AddSocket(webSocket, _dataHandler);
            }
            else
            { 
                await _next(httpContext);
            }
        }
    }
}
