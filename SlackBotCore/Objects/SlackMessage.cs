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
        [GetOnlyJsonProperty]
        private SlackBotApi api;

        [JsonIgnore]
        [GetOnlyJsonProperty]
        public SlackChannel Channel;

        [JsonIgnore]
        [GetOnlyJsonProperty]
        public SlackUser User;

        [JsonIgnore]
        [GetOnlyJsonProperty]
        public DateTime Timestamp;

        [JsonProperty("ts")]
        public string Id;

        [JsonProperty("text")]
        public string Text;
        
        [JsonProperty("attachments")]
        public List<SlackAttachment> Attachments;

        public SlackMessage() { }

        public SlackMessage(SlackBotApi api, string id, string text, SlackChannel channel, SlackUser user, DateTime? timestamp = null, params SlackAttachment[] attachments)
        {
            this.api = api;
            Id = id;
            Text = text;
            Channel = channel;
            User = user;
            Timestamp = timestamp ?? DateTime.UtcNow;
            Attachments = new List<SlackAttachment>(attachments);
        }
        
        public void SetApi(SlackBotApi api)
        {
            this.api = api;
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
