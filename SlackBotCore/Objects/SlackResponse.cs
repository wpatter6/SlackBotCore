using Newtonsoft.Json;

namespace SlackBotCore.Objects
{
    public class SlackResponse
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }
    }
}
