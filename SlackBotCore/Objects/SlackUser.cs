using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlackBotCore.Objects
{
    public class SlackUser : SlackBaseApiObject
    {
        public string Id;
        public string Name;

        public SlackUser() { }

        public SlackUser(SlackBotApi api)
        {
            SetApi(api);
        }

        public async Task<SlackEphemeral> SendEphemeralAsync(SlackChannel channel, string text, params SlackAttachment[] attachments)
        {
            return await Api.SendEphemeralAsync(this, channel, text, attachments);
        }
    }
}
