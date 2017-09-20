using SlackBotFull.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotFull.EventObjects
{
    public class MessageReceivedEventArgs : UserDataReceivedEventArgs
    {
        public readonly string Message;

        public MessageReceivedEventArgs(string message, SlackUser user, SlackChannel channel, SlackTeam team)
            : base(user, channel, team)
        {
            Message = message;
        }
    }
}
