using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotCore.Objects
{
    public class SlackFileComment : SlackBaseApiObject
    {
        public string Id;

        public string Text;

        public SlackFileComment(SlackBotApi api, string id, string text = null)
        {
            SetApi(api);
            Id = id;
            Text = text;
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
