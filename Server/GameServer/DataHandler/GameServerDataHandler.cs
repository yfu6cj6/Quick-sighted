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
        private ILogger<GameServerDataHandler> _logger;

        public GameServerDataHandler(ILogger<GameServerDataHandler> logger)
        {
            _logger = logger;
        }

        public void Receive(string id, WebSocket webSocket, string requestContent)
        {
            _logger.LogInformation(requestContent);
        }
    }
}
