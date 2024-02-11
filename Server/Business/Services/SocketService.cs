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

   /*  public async Task SendNotification(Notification notification)
    {
        var socket = 
    }

    WebSocket IDictionary<string, WebSocket>.this[string key] { get => Users[key]; set =>
    Users[key] = value;
    }

    ICollection<string> IDictionary<string, WebSocket>.Keys => Users.Keys;

    ICollection<WebSocket> IDictionary<string, WebSocket>.Values => Users.Values;

    int ICollection<KeyValuePair<string, WebSocket>>.Count => Users.Count;

    bool ICollection<KeyValuePair<string, WebSocket>>.IsReadOnly => false;

    void IDictionary<string, WebSocket>.Add(string key, WebSocket value)
    {
        Users[key] = value;
    }

    void ICollection<KeyValuePair<string, WebSocket>>.Add(KeyValuePair<string, WebSocket> item)
    {
        Users[item.Key] = item.Value;
    }

    void ICollection<KeyValuePair<string, WebSocket>>.Clear()
    {
        Users.Clear();
    }

    bool ICollection<KeyValuePair<string, WebSocket>>.Contains(KeyValuePair<string, WebSocket> item)
    {
        return Users.Contains(item);
    }

    bool IDictionary<string, WebSocket>.ContainsKey(string key)
    {
        return Users.ContainsKey(key);
    }

    void ICollection<KeyValuePair<string, WebSocket>>.CopyTo(KeyValuePair<string, WebSocket>[] array, int arrayIndex)
    {
        foreach(var kv in Users)
        {
            array[arrayIndex++] = kv;
        }
    }

    IEnumerator<KeyValuePair<string, WebSocket>> IEnumerable<KeyValuePair<string, WebSocket>>.GetEnumerator()
    {
        return Users.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Users.GetEnumerator();
    }

    bool IDictionary<string, WebSocket>.Remove(string key)
    {
        return Users.Remove(key,out _);
    }

    bool ICollection<KeyValuePair<string, WebSocket>>.Remove(KeyValuePair<string, WebSocket> item)
    {
       return Users.Remove(item.Key,out _);
    }

    bool IDictionary<string, WebSocket>.TryGetValue(string key, out WebSocket value)
    {
        return Users.TryGetValue(key, out value);
    } */
}