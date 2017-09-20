using SlackBotFull.EventObjects;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using SlackBotFull.Objects;

namespace SlackBotFull
{
    public class SlackBotMessageSocketConnection
    {
        private ClientWebSocket Socket;
        public async Task ConnectToChat(Uri uri)
        {
            int failCount = 0, maxFails = 5, seconds = 2;

            while (failCount < maxFails)
            {
                try
                {
                    //var wsUri = await GetWebSocketData(uri);

                    Socket = new ClientWebSocket();
                    await Socket.ConnectAsync(uri, CancellationToken.None);

                    var receiveBytes = new byte[4096];
                    var receiveBuffer = new ArraySegment<byte>(receiveBytes);

                    while (Socket.State == WebSocketState.Open)
                    {
                        var receivedMessage = await Socket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                        if (receivedMessage.MessageType == WebSocketMessageType.Close)
                        {
                            await
                                Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing websocket",
                                    CancellationToken.None);
                        }
                        else
                        {
                            var messageBytes = receiveBuffer.Skip(receiveBuffer.Offset).Take(receivedMessage.Count).ToArray();

                            var rawMessage = new UTF8Encoding().GetString(messageBytes);
                            OnDataReceived(JObject.Parse(rawMessage));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    failCount++;
                    Thread.Sleep(seconds * 1000);
                }
            }
        }

        public async Task<dynamic> GetChatWebSocketData(Uri uri)
        {
            var result = await new HttpClient().GetAsync(uri.ToString());

            return JObject.Parse(await result.Content.ReadAsStringAsync());
        }

        public async Task<SlackMessage> SendMessage(string message, SlackChannel channel, SlackUser user)
        {
            var outbound = new
            {
                id = Guid.NewGuid().ToString(),
                type = "message",
                channel = channel,
                text = message
            };

            var outboundBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(outbound));
            var outboundBuffer = new ArraySegment<byte>(outboundBytes);

            await SendData(outboundBuffer);

            return new SlackMessage(message, channel, user, DateTime.UtcNow);
        }

        public async Task SendData(ArraySegment<byte> data)
        {
            await Socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        protected virtual void OnDataReceived(dynamic e)
        {
            DataReceived?.Invoke(this, e);
        }

        public event DataReceivedEventHandler DataReceived;
        public delegate void DataReceivedEventHandler(object sender, dynamic e);
    }
}
