using SlackBotFull.EventObjects;
using SlackBotFull.Objects;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Web;

namespace SlackBotFull
{
    public class SlackBot
    {
        private string urlBase = "https://slack.com/api/";
        private SlackBotMessageSocketConnection messageConnection;
        private Dictionary<BotApiCommands, string> CommandDictionary = new Dictionary<BotApiCommands, string>(new KeyValuePair<BotApiCommands, string>[]
        {
            new KeyValuePair<BotApiCommands, string>(BotApiCommands.Message, "rtm.start")
        });


        private string _token;
        private dynamic _teamData;

        public SlackUser User { get; private set; }

        public SlackTeam Team { get; private set; }

        public SlackBot(string token)
        {
            _token = token;
            messageConnection = new SlackBotMessageSocketConnection();
            messageConnection.DataReceived += Connection_DataReceived;
            Task.WaitAll(Connect());
        }
        
        public async Task SendMessage(string message, SlackChannel channel, SlackUser user = null)
        {
            if (user == null) user = User;
            await messageConnection.SendMessage(message, channel, user);
        }

        private async Task Connect()
        {
            var messageUri = GetApiUri(BotApiCommands.Message, new KeyValuePair<string, string>("token", _token));
            _teamData = await messageConnection.GetChatWebSocketData(messageUri);

            User = new SlackUser()
            {
                Id = _teamData.self.Value<string>("id"),
                Name = _teamData.self.Value<string>("name")
            };

            Team = SlackTeam.FromData(_teamData);

            var url = _teamData["url"];

            if (url == null) throw new Exception("Url not returned from slack api.");

            await messageConnection.ConnectToChat(new Uri(url.ToString()));
        }

        private void Connection_DataReceived(object sender, dynamic data)
        {
            var type = data.type.ToString();

            switch (type)
            {
                case "user_typing"://todo
                    break;
                case "presence_change"://todo
                    break;
                case "message":

                    //var args = new MessageReceivedEventArgs(data.message.ToString(), data
                    OnMessageReceived(null);
                    break;
            }
        }

        private Uri GetApiUri(BotApiCommands commandType, params KeyValuePair<string, string>[] querystringParameters)
        {
            var builder = new UriBuilder(string.Format("{0}{1}", urlBase, CommandDictionary[commandType]));

            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach(var param in querystringParameters)
            {
                query[param.Key] = param.Value;
            }
            builder.Query = query.ToString();

            return builder.Uri;
        }


        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
        public delegate void UserDataReceivedEventHandler(object sender, UserDataReceivedEventArgs e);

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
        public event MessageReceivedEventHandler MessageReceived;
        
        protected virtual void OnUserTyping(UserDataReceivedEventArgs e)
        {
            UserTyping?.Invoke(this, e);
        }
        public event UserDataReceivedEventHandler UserTyping;
    }

    public enum BotApiCommands
    {
        Message,
        Delete
    }
}