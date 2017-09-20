using SlackBotFull.Objects;
using System;

namespace SlackBotFull.EventObjects
{
    public class UserDataReceivedEventArgs : EventArgs
    {
        public readonly SlackUser User;
        public readonly SlackChannel Channel;
        public readonly SlackTeam Team;

        public UserDataReceivedEventArgs(SlackUser user, SlackChannel channel, SlackTeam team)
        {
            User = user;
            Channel = channel;
            Team = team;
        }
    }
}
