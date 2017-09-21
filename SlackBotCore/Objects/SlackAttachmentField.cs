using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotCore.Objects
{
    public class SlackAttachmentField
    {
        [JsonProperty("title")]
        public string Title;
        [JsonProperty("value")]
        public string Value;
        [JsonProperty("short")]
        public bool IsShort;

        public SlackAttachmentField(string title, string value, bool isShort = false)
        {
            Title = title;
            Value = value;
            IsShort = isShort;
        }
    }
}
