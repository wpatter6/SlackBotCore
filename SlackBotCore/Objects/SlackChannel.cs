using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackBotCore.Objects
{
    public class SlackChannel : SlackBaseApiObject
    {
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

        public SlackChannel(SlackBotApi api)
        {
            SetApi(api);
        }

        public async Task<SlackMessage> SendMessageAsync(string text, params SlackAttachment[] attachments)
        {
            return await Api.SendMessageAsync(this, Team.BotUser, text, attachments);
        }

        public async Task<SlackEphemeral> SendEphemeralAsync(SlackUser user, string text, params SlackAttachment[] attachments)
        {
            return await Api.SendEphemeralAsync(user, this, text, attachments);
        }
    }
}
