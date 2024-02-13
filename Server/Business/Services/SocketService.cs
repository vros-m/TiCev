using System.Collections;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Business.Services;


public class SocketService
{
    ConcurrentDictionary<string, WebSocket> Users { get; set; } = new ConcurrentDictionary<string, WebSocket>();

    public async Task HandleConnection(HttpContext context)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        string userId = context.Request.Query["userId"]!;
        Users[userId] = webSocket;
        byte[] buffer = new byte[1024];
        while(webSocket.State==WebSocketState.Open)
            await webSocket.ReceiveAsync(buffer, CancellationToken.None);

    }

    public async Task NotifyUser(Notification notification)
    {
        Users.TryGetValue(notification.RecipientId, out WebSocket? socket);
        if(socket!=null && socket.State==WebSocketState.Open)
        {
            string msg = JsonSerializer.Serialize(notification);
            await socket.SendAsync(Encoding.UTF8.GetBytes(msg),WebSocketMessageType.Text,true,CancellationToken.None);
        }
    }

}