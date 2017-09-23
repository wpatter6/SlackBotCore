using Newtonsoft.Json;
using SlackBotCore.Objects.JsonHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotCore.Objects
{
    public class SlackMessage : SlackBaseApiObject
    {
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

        public SlackMessage() { }

        public SlackMessage(SlackBotApi api, string id, string text, SlackChannel channel, SlackUser user, DateTime? timestamp = null, params SlackAttachment[] attachments)
        {
            SetApi(api);
            Id = id;
            Text = text;
            Channel = channel;
            User = user;
            Timestamp = timestamp ?? DateTime.UtcNow;
            Attachments = new List<SlackAttachment>(attachments);
        }
        
        public async Task<SlackResponse> DeleteAsync()
        {
            return await Api.DeleteMessageAsync(Channel, Id);
        }

        public async Task<SlackMessage> UpdateAsync(string text = null, params SlackAttachment[] attachments)
        {
            Text = text ?? Text;

            Attachments = new List<SlackAttachment>(attachments);

            var result = await Api.UpdateMessageAsync(this);
            Id = result.Id;

            return this;
        }

        public async Task<SlackResponse> AddReactionAsync(string emojiName)
        {
            return await Api.AddReactionAsync(this, emojiName);
        }

        public async Task<SlackResponse> RemoveReactionAsync(string emojiName)
        {
            return await Api.RemoveReactionAsync(this, emojiName);
        }

        public async Task<SlackResponse> AddStarAsync()
        {
            return await Api.AddStarAsync(this);
        }

        public async Task<SlackResponse> RemoveStarAsync()
        {
            return await Api.RemoveStarAsync(this);
        }
    }
}
