using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketManager
{
    private static WebSocketManager _instance;

    public static WebSocketManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new WebSocketManager();
            }
            return _instance;
        }
    }

    private WebSocketManager() { }

    private readonly ConcurrentDictionary<string, ClientWebSocket> _webSockets = new ConcurrentDictionary<string, ClientWebSocket>();

    public async void AddSocket(string connectUrl, IConnectDataHandler dataHandler)
    {
        var webSocket = new ClientWebSocket();
        await webSocket.ConnectAsync(new Uri(connectUrl), new CancellationToken());
        var id = CreateConnectionId();
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
                    OnDisconnect(id, dataHandler);
                    return;
                }

                dataHandler.Receive(id, webSocket, Encoding.UTF8.GetString(buffer, 0, result.Count));
            }
            catch
            {
                OnDisconnect(id, dataHandler);
            }
        }
    }

    private void OnDisconnect(string id, IConnectDataHandler dataHandler)
    {
        _webSockets.TryRemove(id, out ClientWebSocket webSocket);
        if (webSocket != null)
        {
            webSocket.Abort();
            webSocket.Dispose();
        }
        dataHandler.OnDisconnect(id);
    }

    public async Task RemoveSocket(string id, string closeStatusDescription, Action<string, string> callBack = null)
    {
        Debug.Log("RemoveSocket");
        _webSockets.TryRemove(id, out ClientWebSocket webSocket);

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
        callBack?.Invoke(id, closeStatusDescription);
    }

    public async void SendMessageAsync(ClientWebSocket webSocket, string message)
    {
        if (webSocket.State != WebSocketState.Open)
            return;

        byte[] buffer = Encoding.UTF8.GetBytes(message);

        await webSocket.SendAsync(buffer: new ArraySegment<byte>(array: buffer, offset: 0, count: buffer.Length),
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);
    }

    private string CreateConnectionId()
    {
        return Guid.NewGuid().ToString();
    }
}
