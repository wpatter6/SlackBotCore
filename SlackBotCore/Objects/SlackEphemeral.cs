using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SlackBotCore.Objects
{
    public class SlackEphemeral : SlackResponse
    {
        [JsonProperty("message_ts")]
        public string Id { get; set; }

        [JsonIgnore]
        public SlackChannel Channel { get; set; }

        [JsonIgnore]
        public SlackUser User { get; set; }
    }
}
