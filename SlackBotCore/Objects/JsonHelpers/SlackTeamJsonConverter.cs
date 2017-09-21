using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackBotCore.Objects.JsonHelpers
{
    public class SlackTeamJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(SlackTeam));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var team = (SlackTeam)value;
            var t = JToken.FromObject(team.Id);
            t.WriteTo(writer);
        }

        public override bool CanRead => false;
    }
}
