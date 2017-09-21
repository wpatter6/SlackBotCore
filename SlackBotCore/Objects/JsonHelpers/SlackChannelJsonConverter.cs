using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotCore.Objects.JsonHelpers
{
    public class SlackChannelJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(SlackChannel));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var channel = (SlackChannel)value;
            var t = JToken.FromObject(channel.Id);
            t.WriteTo(writer);
        }

        public override bool CanRead => false;
    }
}
