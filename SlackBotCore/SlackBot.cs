using SlackBotCore.EventObjects;
using SlackBotCore.Objects;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Web;

namespace SlackBotCore
{
    public class SlackBot
    {
        private SlackBotSocket socket;
        private SlackBotApi api;

        private dynamic teamData;

        public SlackUser BotUser { get; private set; }

        public SlackTeam Team { get; private set; }

        //public SlackBot(string clientId, string clientSecret)
        //{
        //    api = new SlackBotApi(clientId, clientSecret);
        //    socket = new SlackBotSocket();
        //    socket.DataReceived += Socket_DataReceived;
        //}

        public SlackBot(string token)
        {
            api = new SlackBotApi(token);
            socket = new SlackBotSocket();
            socket.DataReceived += Socket_DataReceived;
        }
        
        public async Task<SlackMessage> SendMessageAsync(SlackChannel channel, string message, params SlackAttachment[] attachments)
        {
            return await api.SendMessageAsync(channel, BotUser, message, attachments);
        }

        public async Task<IDisposable> Connect()
        {
            teamData = await api.GetConnectionAsync();



            Team = SlackTeam.FromData(teamData, api);

            var url = teamData["url"];

            if (url == null) throw new Exception("Url not returned from slack api.");
            
            return await socket.OpenSocket(new Uri(url.ToString()));
        }

        public async Task Disconnect()
        {
            await socket.CloseSocket();
        }

        private void Socket_DataReceived(object sender, dynamic data)
        {
            var type = data.Value<string>("type");
            
            switch (type)//todo more from https://api.slack.com/rtm
            {
                case "message":
                case "message.channels":
                case "message.groups":
                case "message.im":
                case "message.mpim":
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(MakeMessageFromData(data)));
                    break;
                case "reaction_added":
                    ReactionAdded?.Invoke(this, GetReactionAddedEventArgs(data));
                    break;
                case "reaction_removed":
                    ReactionRemoved?.Invoke(this, GetReactionAddedEventArgs(data));
                    break;
                case "star_added":
                    StarAdded?.Invoke(this, GetReactionAddedEventArgs(data));
                    break;
                case "star_removed":
                    StarRemoved?.Invoke(this, GetReactionAddedEventArgs(data));
                    break;
                case "pin_added":
                    PinAdded?.Invoke(this, GetReactionAddedEventArgs(data));
                    break;
                case "pin_removed":
                    PinRemoved?.Invoke(this, GetReactionAddedEventArgs(data));
                    break;
                case "team_join":
                    UserJoined?.Invoke(this, new UserDataReceivedEventArgs(Team.Users.FirstOrDefault(x => x.Id == data.Value<string>("user")), null, Team));
                    break;
                case "user_typing":
                    UserTyping?.Invoke(this, new UserDataReceivedEventArgs(Team.Users.FirstOrDefault(x => x.Id == data.Value<string>("user")),
                        Team.Channels.FirstOrDefault(x => x.Id == data.Value<string>("channel")), Team));
                    break;
                case "presence_change":
                    UserPresenceChanged?.Invoke(this, new UserPresenceChangeEventArgs(data.Value<string>("presence"),
                        Team.Users.FirstOrDefault(x => x.Id == data.Value<string>("user"))));
                    break;
            }
        }

        private SlackMessage MakeMessageFromData(dynamic data, string userid = null)
        {
            var uid = userid ?? data.Value<string>("user");
            return new SlackMessage(api, data.Value<string>("ts"), data.Value<string>("text"),
                Team.Channels.FirstOrDefault(x => x.Id == data.Value<string>("channel")),
                Team.Users.FirstOrDefault(x => x.Id == uid));
        }

        private ReactionAddedEventArgs GetReactionAddedEventArgs(dynamic data)
        {
            var type = data.Value<string>("type");
            var item = data.item;
            ReactionAddedEventArgs args;
            var reaction = data["reaction"]?.ToString() ?? type;
            switch (item.Value<string>("type"))
            {
                case "file":
                    args = new ReactionAddedEventArgs(reaction,
                        Team.Users.FirstOrDefault(x => x.Id == data.Value<string>("user")),
                        new SlackFile(api, data.item.Value<string>("file"),
                        Team.Users.FirstOrDefault(x => x.Id == data.Value<string>("item_user"))));
                    break;
                case "file_comment":
                    args = new ReactionAddedEventArgs(reaction,
                        Team.Users.FirstOrDefault(x => x.Id == data.Value<string>("user")),
                        new SlackFileComment(api, data.item.Value<string>("file_comment")));
                    break;
                default:
                    args = new ReactionAddedEventArgs(reaction,
                        Team.Users.FirstOrDefault(x => x.Id == data.Value<string>("user")),
                        MakeMessageFromData(data.item, data.Value<string>("item_user")));
                    break;
            }
            return args;
        }

        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
        public delegate void UserDataReceivedEventHandler(object sender, UserDataReceivedEventArgs e);
        public delegate void UserPresenceChangedEventHandler(object sender, UserPresenceChangeEventArgs e);
        public delegate void ReactionAddedEventHandler(object sender, ReactionAddedEventArgs e);

        public event MessageReceivedEventHandler MessageReceived;
        public event UserDataReceivedEventHandler UserTyping;
        public event UserDataReceivedEventHandler UserJoined;
        public event UserPresenceChangedEventHandler UserPresenceChanged;
        public event ReactionAddedEventHandler ReactionAdded;
        public event ReactionAddedEventHandler ReactionRemoved;
        public event ReactionAddedEventHandler StarAdded;
        public event ReactionAddedEventHandler StarRemoved;
        public event ReactionAddedEventHandler PinAdded;
        public event ReactionAddedEventHandler PinRemoved;
    }
}