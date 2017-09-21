using SlackBotCore.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotCore.EventObjects
{
    public class UserPresenceChangeEventArgs : EventArgs
    {
        public readonly string Presence;
        public readonly SlackUser User;

        public UserPresenceChangeEventArgs(string presence, SlackUser user)
        {
            User = user;
            Presence = presence;
        }
    }
}
