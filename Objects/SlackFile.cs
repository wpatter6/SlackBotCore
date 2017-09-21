using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotCore.Objects
{
    public class SlackFile
    {
        public readonly string Id;
        public readonly SlackUser User;

        public SlackFile(string id, SlackUser user)
        {
            Id = id;
            User = user;
        }
    }
}
