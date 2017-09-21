using SlackBotCore.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotCore.EventObjects
{
    public class MessageReceivedEventArgs : UserDataReceivedEventArgs
    {
        public readonly SlackMessage Message;
        public readonly string Content;

        public MessageReceivedEventArgs(SlackMessage message)
            : base(message.User, message.Channel, message.Channel.Team)
        {
            Message = message;
            Content = message.Text;
        }
    }
}
