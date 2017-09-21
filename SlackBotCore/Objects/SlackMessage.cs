using Newtonsoft.Json;
using SlackBotCore.Objects.JsonHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotCore.Objects
{
    public class SlackMessage
    {
        [JsonIgnore]
        private SlackBotApi api;

        [JsonIgnore]
        public SlackChannel Channel;

        [JsonIgnore]
        public SlackUser User;

        [JsonIgnore]
        public DateTime Timestamp;

        [JsonProperty("ts")]
        public string Id;

        [JsonProperty("text")]
        public string Text;
        
        [JsonProperty("attachments")]
        public List<SlackAttachment> Attachments;

        [JsonProperty("mrkdwn")]
        public bool EnableMarkdown;

        public SlackMessage(SlackBotApi api, string id, string text, SlackChannel channel, SlackUser user, bool enableMarkdown = true, DateTime? timestamp = null, params SlackAttachment[] attachments)
        {
            this.api = api;
            Id = id;
            Text = text;
            Channel = channel;
            User = user;
            Timestamp = timestamp ?? DateTime.UtcNow;
            Attachments = new List<SlackAttachment>(attachments);
            EnableMarkdown = enableMarkdown;
        }
        
        public async Task DeleteAsync()
        {
            await api.DeleteMessageAsync(Channel.Id, Id);
        }

        public async Task<SlackMessage> UpdateAsync(string text = null, params SlackAttachment[] attachments)
        {
            Text = text ?? Text;

            Attachments = new List<SlackAttachment>(attachments);

            var result = await api.UpdateMessageAsync(this);
            Id = result.Id;

            return this;
        }
    }
}
