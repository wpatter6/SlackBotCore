using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SlackBotCore.Objects;
using SlackBotCore.Objects.JsonHelpers;
using Newtonsoft.Json;

namespace SlackBotCore
{
    public enum ApiCommands
    {
        Connect,
        Message,
        Delete,
        Update,
        Token
    }

    public class SlackBotApi
    {
        public SlackBotApi(string clientId, string clientSecret)
        {

        }

        public SlackBotApi(string token)
        {
            this.token = token;
        }

        #region public
        public async Task<SlackMessage> SendMessageAsync(SlackMessage message)
        {
            return await SendMessageAsync(message.Channel, message.Text, message.EnableMarkdown, message.Attachments.ToArray());
        }

        public async Task<SlackMessage> SendMessageAsync(SlackChannel channel, string text, bool enableMarkdown = true, params SlackAttachment[] attachments)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("channel", channel.Id));
            parameters.Add(new KeyValuePair<string, string>("text", text));
            parameters.Add(new KeyValuePair<string, string>("mrkdwn", enableMarkdown.ToString().ToLower()));
            parameters.Add(new KeyValuePair<string, string>("as_user", "true"));

            if (attachments.Length > 0)
                parameters.Add(new KeyValuePair<string, string>("attachments", JArray.FromObject(attachments).ToString(Formatting.None)));

            var result = await GetApiData<SlackMessage>(ApiCommands.Message, parameters.ToArray());
            result.Timestamp = DateTime.UtcNow;
            result.Channel = channel;
            return result;
        }

        public async Task<SlackMessage> UpdateMessageAsync(SlackMessage message)
        {
            return await UpdateMessageAsync(message.Channel, message.Id, message.Text, message.EnableMarkdown, message.Attachments.ToArray());
        }

        public async Task<SlackMessage> UpdateMessageAsync(SlackChannel channel, string ts, string text, bool enableMarkdown = true, params SlackAttachment[] attachments)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("channel", channel.Id));
            parameters.Add(new KeyValuePair<string, string>("text", text));
            parameters.Add(new KeyValuePair<string, string>("mrkdwn", enableMarkdown.ToString().ToLower()));
            parameters.Add(new KeyValuePair<string, string>("as_user", "true"));

            if (attachments.Length > 0)
                parameters.Add(new KeyValuePair<string, string>("attachments", JArray.FromObject(attachments).ToString(Formatting.None)));

            var result = await GetApiData<SlackMessage>(ApiCommands.Update, parameters.ToArray());
            result.Channel = channel;
            return result;
        }

        public async Task DeleteMessageAsync(SlackMessage message)
        {
            await DeleteMessageAsync(message.Channel.Id, message.Id);
        }

        public async Task DeleteMessageAsync(string channel, string ts)
        {
            await GetApiData(ApiCommands.Delete,
                new KeyValuePair<string, string>("channel", channel),
                new KeyValuePair<string, string>("ts", ts));
        }

        public async Task<JObject> GetConnectionAsync()
        {
            return await GetApiData(ApiCommands.Connect);
        }
        #endregion

        #region private
        private string token;
        private string urlBase = "https://slack.com/api/";
        

        private Dictionary<ApiCommands, string> CommandDictionary = new Dictionary<ApiCommands, string>(new KeyValuePair<ApiCommands, string>[]
        {
            new KeyValuePair<ApiCommands, string>(ApiCommands.Connect, "rtm.start"),
            new KeyValuePair<ApiCommands, string>(ApiCommands.Message, "chat.postMessage"),
            new KeyValuePair<ApiCommands, string>(ApiCommands.Delete, "chat.delete"),
            new KeyValuePair<ApiCommands, string>(ApiCommands.Update, "chat.update"),
            new KeyValuePair<ApiCommands, string>(ApiCommands.Token, "oauth.access")
        });

        private async Task<JObject> GetApiData(ApiCommands commandType, params KeyValuePair<string, string>[] queryParameters)
        {
            return await GetApiData<JObject>(commandType, queryParameters);
        }

        private async Task<TResult> GetApiData<TResult>(ApiCommands commandType, params KeyValuePair<string, string>[] queryParameters)
        {
            return await GetApiData<TResult>(GetApiUri(commandType, queryParameters));
        }

        private async Task<JObject> GetApiData(Uri uri)
        {
            return await GetApiData<JObject>(uri);
        }

        private async Task<TResult> GetApiData<TResult>(Uri uri)
        {
            var result = await new HttpClient().GetAsync(uri.ToString());
            var json = await result.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(json);

            if (jobj["ok"] != null && !jobj.Value<bool>("ok"))
                throw new Exception($"An error occurred calling the slack API: {json}");

            return jobj.ToObject<TResult>();
        }

        private IgnorableSerializerContractResolver jsonIgnores;

        private IgnorableSerializerContractResolver GetIgnores()
        {
            if (jsonIgnores != null) return jsonIgnores;

            jsonIgnores = new IgnorableSerializerContractResolver();
            jsonIgnores.Ignore(typeof(SlackMessage), "Channel", "User", "channel", "user");
            jsonIgnores.Ignore(typeof(SlackChannel), "Team", "team");
            jsonIgnores.Ignore(typeof(SlackTeam), "Channels", "channels", "Users", "users");

            return jsonIgnores;
        }

        private Uri GetApiUri(ApiCommands commandType, params KeyValuePair<string, string>[] queryParameters)
        {
            var builder = new UriBuilder(string.Format("{0}{1}", urlBase, CommandDictionary[commandType]));

            var query = HttpUtility.ParseQueryString(builder.Query);
            query["token"] = token;
            foreach (var param in queryParameters)
            {
                query[param.Key] = param.Value;
            }
            builder.Query = query.ToString();

            return builder.Uri;
        }
        #endregion
    }
}
