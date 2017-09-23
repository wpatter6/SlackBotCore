using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel;
using SlackBotCore.Objects;
using SlackBotCore.Objects.JsonHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace SlackBotCore
{
    public enum ApiCommands
    {
        [Description("rtm.start")]
        Connect,
        [Description("chat.postMessage")]
        Message,
        [Description("chat.postEphemeral")]
        Ephemeral,
        [Description("chat.delete")]
        Delete,
        [Description("chat.update")]
        Update,
        [Description("oauth.access")]
        Token,
        [Description("reactions.add")]
        AddReaction,
        [Description("reactions.remove")]
        RemoveReaction,
        [Description("stars.add")]
        AddStar,
        [Description("stars.remove")]
        RemoveStar
    }

    public class SlackBotApi
    {
        public SlackBotApi(string token)
        {
            this.token = token;
        }

        #region public
        public async Task<JObject> GetConnectionAsync()
        {
            return await GetApiData(ApiCommands.Connect);
        }

        #region messages
        public async Task<SlackMessage> SendMessageAsync(SlackMessage message)
        {
            return await SendMessageAsync(message.Channel, message.User, message.Text, message.Attachments.ToArray());
        }

        public async Task<SlackMessage> SendMessageAsync(SlackChannel channel, SlackUser user, string text, params SlackAttachment[] attachments)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("channel", channel.Id));
            parameters.Add(new KeyValuePair<string, string>("text", text));
            parameters.Add(new KeyValuePair<string, string>("as_user", "true"));

            if (attachments.Length > 0)
                parameters.Add(new KeyValuePair<string, string>("attachments", JArray.FromObject(attachments).ToString(Formatting.None)));
            
            var result = await GetApiDataProperty<SlackMessage>(ApiCommands.Message, "message", new string[] { "user", "channel" }, parameters.ToArray());

            if (result == default(SlackMessage)) return default(SlackMessage);
            result.SetApi(this);
            result.Timestamp = DateTime.UtcNow;
            result.Channel = channel;
            result.User = user;
            return result;
        }

        public async Task<SlackMessage> UpdateMessageAsync(SlackMessage message)
        {
            return await UpdateMessageAsync(message.Channel, message.Id, message.Text, message.Attachments.ToArray());
        }

        public async Task<SlackMessage> UpdateMessageAsync(SlackChannel channel, string ts, string text, params SlackAttachment[] attachments)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("ts", ts));
            parameters.Add(new KeyValuePair<string, string>("channel", channel.Id));
            parameters.Add(new KeyValuePair<string, string>("text", text));
            parameters.Add(new KeyValuePair<string, string>("as_user", "true"));

            if (attachments.Length > 0)
                parameters.Add(new KeyValuePair<string, string>("attachments", JArray.FromObject(attachments).ToString(Formatting.None)));

            var result = await GetApiData<SlackMessage>(ApiCommands.Update, null, parameters.ToArray());
            result.Channel = channel;
            return result;
        }

        public async Task<SlackResponse> DeleteMessageAsync(SlackMessage message)
        {
            return await DeleteMessageAsync(message.Channel, message.Id);
        }

        public async Task<SlackResponse> DeleteMessageAsync(SlackChannel channel, string ts)
        {
            return await GetApiData<SlackResponse>(ApiCommands.Delete, null,
                new KeyValuePair<string, string>("channel", channel.Id),
                new KeyValuePair<string, string>("ts", ts));
        }

        public async Task<SlackEphemeral> SendEphemeralAsync(SlackUser user, SlackChannel channel, string text, params SlackAttachment[] attachments)
        {
            var result = await GetApiData<SlackEphemeral>(ApiCommands.Ephemeral, null,
                new KeyValuePair<string, string>("user", user.Id),
                new KeyValuePair<string, string>("channel", channel.Id),
                new KeyValuePair<string, string>("text", text));

            result.User = user;
            result.Channel = channel;

            return result;
        }
        #endregion

        #region stars/reactions
        public async Task<SlackResponse> AddReactionAsync(SlackMessage message, string emojiName)
        {
            return await GetApiData<SlackResponse>(ApiCommands.AddReaction, null,
                new KeyValuePair<string, string>("channel", message.Channel.Id),
                new KeyValuePair<string, string>("timestamp", message.Id),
                new KeyValuePair<string, string>("name", emojiName));
        }

        public async Task<SlackResponse> AddReactionAsync(SlackFile file, string emojiName)
        {
            return await GetApiData<SlackResponse>(ApiCommands.AddReaction, null,
                new KeyValuePair<string, string>("file", file.Id),
                new KeyValuePair<string, string>("name", emojiName));
        }

        public async Task<SlackResponse> AddReactionAsync(SlackFileComment fileComment, string emojiName)
        {
            return await GetApiData<SlackResponse>(ApiCommands.AddReaction, null,
                new KeyValuePair<string, string>("file_comment", fileComment.Id),
                new KeyValuePair<string, string>("name", emojiName));
        }

        public async Task<SlackResponse> RemoveReactionAsync(SlackMessage message, string emojiName)
        {
            return await GetApiData<SlackResponse>(ApiCommands.RemoveReaction, null,
                new KeyValuePair<string, string>("channel", message.Channel.Id),
                new KeyValuePair<string, string>("timestamp", message.Id),
                new KeyValuePair<string, string>("name", emojiName));
        }

        public async Task<SlackResponse> RemoveReactionAsync(SlackFile file, string emojiName)
        {
            return await GetApiData<SlackResponse>(ApiCommands.RemoveReaction, null,
                new KeyValuePair<string, string>("file", file.Id),
                new KeyValuePair<string, string>("name", emojiName));
        }

        public async Task<SlackResponse> RemoveReactionAsync(SlackFileComment fileComment, string emojiName)
        {
            return await GetApiData<SlackResponse>(ApiCommands.RemoveReaction, null,
                new KeyValuePair<string, string>("file_comment", fileComment.Id),
                new KeyValuePair<string, string>("name", emojiName));
        }

        public async Task<SlackResponse> AddStarAsync(SlackMessage message)
        {
            return await GetApiData<SlackResponse>(ApiCommands.AddStar, null,
                new KeyValuePair<string, string>("channel", message.Channel.Id),
                new KeyValuePair<string, string>("timestamp", message.Id));
        }

        public async Task<SlackResponse> AddStarAsync(SlackFile file)
        {
            return await GetApiData<SlackResponse>(ApiCommands.AddStar, null,
                new KeyValuePair<string, string>("file", file.Id));
        }

        public async Task<SlackResponse> AddStarAsync(SlackFileComment fileComment)
        {
            return await GetApiData<SlackResponse>(ApiCommands.AddStar, null,
                new KeyValuePair<string, string>("file_comment", fileComment.Id));
        }

        public async Task<SlackResponse> RemoveStarAsync(SlackMessage message)
        {
            return await GetApiData<SlackResponse>(ApiCommands.RemoveStar, null,
                new KeyValuePair<string, string>("channel", message.Channel.Id),
                new KeyValuePair<string, string>("timestamp", message.Id));
        }

        public async Task<SlackResponse> RemoveStarAsync(SlackFile file)
        {
            return await GetApiData<SlackResponse>(ApiCommands.RemoveStar, null,
                new KeyValuePair<string, string>("file", file.Id));
        }

        public async Task<SlackResponse> RemoveStarAsync(SlackFileComment fileComment)
        {
            return await GetApiData<SlackResponse>(ApiCommands.RemoveStar, null,
                new KeyValuePair<string, string>("file_comment", fileComment.Id));
        }
        #endregion
        #endregion

        #region private
        private string token;
        private string urlBase = "https://slack.com/api/";
        
        private async Task<TResult> GetApiDataProperty<TResult>(ApiCommands commandType, string propertyName, string [] ignoreProperties = null, params KeyValuePair<string, string>[] queryParameters)
        {
            var jobj = await GetApiData(commandType, null, queryParameters);

            if (jobj[propertyName] == null) return default(TResult);

            var jtoken = jobj[propertyName];

            if (ignoreProperties != null)
            {
                foreach (var property in ignoreProperties)
                {
                    var prop = jtoken.Children<JProperty>().FirstOrDefault(x => x.Name == property);
                    if (prop != null)
                    {
                        prop.Remove();
                    }
                }
            }

            return DeserializeJson<TResult>(jtoken);
        }
        
        private async Task<JObject> GetApiData(ApiCommands commandType, string[] ignoreProperties = null, params KeyValuePair<string, string>[] queryParameters)
        {
            return await GetApiData<JObject>(commandType, ignoreProperties, queryParameters);
        }

        private async Task<TResult> GetApiData<TResult>(ApiCommands commandType, string[] ignoreProperties = null, params KeyValuePair<string, string>[] queryParameters)
        {
            return await GetApiData<TResult>(GetApiUri(commandType, queryParameters), ignoreProperties);
        }

        private async Task<JObject> GetApiData(Uri uri, string[] ignoreProperties = null)
        {
            return await GetApiData<JObject>(uri, ignoreProperties);
        }

        private async Task<TResult> GetApiData<TResult>(Uri uri, string[] ignoreProperties = null)
        {
            var result = await new HttpClient().GetAsync(uri.ToString());
            var json = await result.Content.ReadAsStringAsync();
            var jobj = JObject.Parse(json);

            if (jobj["ok"] != null && !jobj.Value<bool>("ok"))
                throw new Exception($"An error occurred calling the slack API: {json}");


            if (ignoreProperties != null)
            {
                foreach (var property in ignoreProperties)
                {
                    var prop = jobj.Children<JProperty>().FirstOrDefault(x => x.Name == property);
                    if (prop != null)
                    {
                        prop.Remove();
                    }
                }
            }

            return DeserializeJson<TResult>(jobj);
        }
        
        private TResult DeserializeJson<TResult>(JToken jobj)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return jobj.ToObject<TResult>(JsonSerializer.Create(settings));
        }

        private Uri GetApiUri(ApiCommands commandType, params KeyValuePair<string, string>[] queryParameters)
        {
            var builder = new UriBuilder(string.Format("{0}{1}", urlBase, GetEnumDescription(commandType)));

            var query = HttpUtility.ParseQueryString(builder.Query);
            query["token"] = token;
            foreach (var param in queryParameters)
            {
                query[param.Key] = param.Value;
            }
            builder.Query = query.ToString();

            return builder.Uri;
        }

        private string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        #endregion
    }
}
