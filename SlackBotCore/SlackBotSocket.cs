using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SlackBotCore
{
    public class SlackBotSocket : IDisposable
    {
        private ClientWebSocket socket;
        private Thread socketThread;

        public async Task<IDisposable> OpenSocket(Uri uri)
        {
            int failCount = 0, maxFails = 3, seconds = 2;
            while (failCount < maxFails)
            {
                try
                {
                    socket = new ClientWebSocket();
                    await socket.ConnectAsync(uri, CancellationToken.None);

                    var bytes = new byte[4096];
                    var buffer = new ArraySegment<byte>(bytes);

                    socketThread = new Thread(() => HandleSocket(this, socket, buffer));
                    socketThread.Start();
                    break;
                }
                catch (Exception e)
                {
                    failCount++;
                    if (failCount == maxFails) throw e;

                    Thread.Sleep(seconds * 1000);
                }
            }
            
            return new SocketDisposer(this);
        }

        public async Task CloseSocket(string description = "SlackBotSocket.CloseSocket")
        {
            if (socket.State == WebSocketState.Open)
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "SlackBotSocket.CloseSocket", CancellationToken.None);

            if (socketThread.IsAlive)
                socketThread.Abort();
        }
        
        public virtual void OnDataReceived(dynamic e)
        {
            DataReceived?.Invoke(this, e);
        }

        public void Dispose()
        {
            Task.WaitAll(CloseSocket());
            socket = null;
        }

        public event DataReceivedEventHandler DataReceived;

        public delegate void DataReceivedEventHandler(object sender, dynamic e);

        private async static void HandleSocket(SlackBotSocket botsocket, ClientWebSocket socket, ArraySegment<byte> buffer)
        {
            var i = 300;
            while(socket.State != WebSocketState.Open)
            {
                Thread.Sleep(10);
                if (--i <= 0) return;
            }
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var received = await socket.ReceiveAsync(buffer, CancellationToken.None);

                    if (received.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "NormalClosure", CancellationToken.None);
                    }
                    else
                    {
                        var messageBytes = buffer.Skip(buffer.Offset).Take(received.Count).ToArray();

                        var rawMessage = new UTF8Encoding().GetString(messageBytes);
                        if (!string.IsNullOrWhiteSpace(rawMessage))
                            botsocket.OnDataReceived(JObject.Parse(rawMessage));
                    }
                }
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException) return;
                else throw e;
            }
        }
    }

    class SocketDisposer : IDisposable
    {
        SlackBotSocket _socket;

        public SocketDisposer(SlackBotSocket socket)
        {
            _socket = socket;
        }

        public void Dispose()
        {
            Task.WaitAll(_socket.CloseSocket());
        }
    }
}