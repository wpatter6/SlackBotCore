using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotCore.Objects
{
    public class SlackChannel
    {
        public string Id;
        public string Name;
        public string Topic;
        public string Purpose;

        public SlackTeam Team;
        public List<SlackUser> Members = new List<SlackUser>();
    }
}
