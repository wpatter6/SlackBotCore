using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;

namespace SlackBotCore.Objects.JsonHelpers
{
    public class ColorJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Color));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (Color)new ColorConverter().ConvertFromString((string)existingValue);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = (Color)value;
            var hex = String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);

            var t = JToken.FromObject(hex);

            t.WriteTo(writer);
        }
    }
}
