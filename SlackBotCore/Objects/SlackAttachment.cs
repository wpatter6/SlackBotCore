using Newtonsoft.Json;
using SlackBotCore.Objects.JsonHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SlackBotCore.Objects
{
    public class SlackAttachment
    {
        [JsonProperty("ts")]
        public string Id;

        [JsonProperty("fallback")]
        public string Fallback;

        [JsonProperty("pretext")]
        public string PreText;

        [JsonProperty("author_name")]
        public string AuthorName;

        [JsonProperty("author_link")]
        public string AuthorLinkUrl;

        [JsonProperty("author_icon")]
        public string AuthorIconUrl;

        [JsonProperty("title")]
        public string Title;

        [JsonProperty("title_link")]
        public string TitleLinkUrl;

        [JsonProperty("text")]
        public string Text;

        [JsonProperty("image_url")]
        public string ImageUrl;

        [JsonProperty("thumb_url")]
        public string ThumbnailUrl;

        [JsonProperty("footer")]
        public string Footer;

        [JsonProperty("footer_icon")]
        public string FooterIconUrl;

        [JsonProperty("color")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color Color;

        [JsonProperty("fields")]
        public List<SlackAttachmentField> Fields = new List<SlackAttachmentField>();
    }
}