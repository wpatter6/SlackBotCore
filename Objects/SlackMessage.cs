using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotCore.Objects
{
    public class SlackMessage
    {
        public string Id;
        public SlackChannel Channel;
        public SlackUser User;
        public string Content;
        public DateTime Timestamp;

        public SlackMessage(string id, string content, SlackChannel channel, SlackUser user, DateTime timestamp)
        {
            Id = id;
            Content = content;
            Channel = channel;
            User = user;
            Timestamp = timestamp;
        }
    }
}
