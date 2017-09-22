using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackBotCore.Objects
{
    public class SlackChannel
    {
        [JsonIgnore]
        private SlackBotApi api;
        [JsonIgnore]
        private SlackUser user;

        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("topic")]
        public string Topic;

        [JsonProperty("purpose")]
        public string Purpose;

        [JsonIgnore]
        public SlackTeam Team;

        [JsonIgnore]
        public List<SlackUser> Members = new List<SlackUser>();

        public SlackChannel(SlackBotApi api, SlackUser user)
        {
            this.api = api;
            this.user = user;
        }

        public async Task<SlackMessage> SendMessageAsync(string text, params SlackAttachment[] attachments)
        {
            return await api.SendMessageAsync(this, user, text, attachments);
        }
    }
}
