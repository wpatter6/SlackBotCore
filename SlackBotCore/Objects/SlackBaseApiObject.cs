using System;
using System.Collections.Generic;
using System.Text;

namespace SlackBotCore.Objects
{
    public abstract class SlackBaseApiObject
    {
        protected SlackBotApi Api;

        internal void SetApi(SlackBotApi api)
        {
            Api = api;
        }
    }
}
