using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotFull.Objects
{
    public class SlackMessage
    {
        public SlackChannel Channel;
        public SlackUser User;
        public string Content;
        public DateTime Timestamp;

        public SlackMessage(string content, SlackChannel channel, SlackUser user, DateTime timestamp)
        {
            Content = content;
            Channel = channel;
            User = user;
            Timestamp = timestamp;
        }
    }
}
