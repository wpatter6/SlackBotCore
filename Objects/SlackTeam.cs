using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SlackBotFull.Objects
{
    public class SlackTeam
    {
        public string Id;
        public string Name;
        public string Domain;

        public List<SlackChannel> Channels = new List<SlackChannel>();
        public List<SlackUser> Users = new List<SlackUser>();

        public static SlackTeam FromData(dynamic data)
        {
            var team = new SlackTeam()
            {
                Id = data.team.Value<string>("id"),
                Name = data.team.Value<string>("name"),
                Domain = data.team.Value<string>("domain")
            };

            foreach(var u in data.users)
            {
                if (!u.Value<bool>("deleted"))
                    team.Users.Add(new SlackUser()
                    {
                        Id = u.Value<string>("id"),
                        Name = u.Value<string>("name")
                    });
            }

            foreach(var c in data.channels)
            {
                if (c == null) continue;
                var channel = new SlackChannel()
                {
                    Id = c.Value<string>("id"),
                    Name = c.Value<string>("name"),
                    Purpose = c["purpose"]?.Value<string>("value"),
                    Topic = c["topic"]?.Value<string>("value"),
                    Team = team
                };

                if(c["members"] != null)
                    foreach(var u in c.members)
                    {
                        var user = team.Users.FirstOrDefault(x => x.Id == u.ToString());
                        if(user != null) channel.Members.Add(user);
                    }

                team.Channels.Add(channel);
            }
            
            return team;
        }
    }
}
