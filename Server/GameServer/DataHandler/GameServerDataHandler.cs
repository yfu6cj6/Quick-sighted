using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace GameServer
{
    public class GameServerDataHandler : IConnectDataHandler
    {
        private readonly WebSocketManager _webSocketManager;
        private ILogger<GameServerDataHandler> _logger;

        public GameServerDataHandler(WebSocketManager webSocketManager, ILogger<GameServerDataHandler> logger)
        {
            _webSocketManager = webSocketManager;
            _logger = logger;
        }

        public void OnConnected(string id, WebSocket webSocket)
        {
            _logger.LogInformation("OnConnected");
            _webSocketManager.SendMessageAsync(webSocket, "World");
        }

        public void OnDisconnect(string id)
        {
            _logger.LogInformation("OnDisconnect");
        }

        public void Receive(string id, WebSocket webSocket, string requestContent)
        {
            _logger.LogInformation(requestContent);
        }
    }
}
