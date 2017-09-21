using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SlackBotCore
{
    public enum ApiCommands
    {
        Connect,
        Message,
        Delete
    }

    public class SlackBotApi
    {
        private string _token;
        public SlackBotApi(string token)
        {
            _token = token;
        }
        private string urlBase = "https://slack.com/api/";

        private Dictionary<ApiCommands, string> CommandDictionary = new Dictionary<ApiCommands, string>(new KeyValuePair<ApiCommands, string>[]
        {
            new KeyValuePair<ApiCommands, string>(ApiCommands.Connect, "rtm.start"),
            new KeyValuePair<ApiCommands, string>(ApiCommands.Message, "chat.postMessage"),
            new KeyValuePair<ApiCommands, string>(ApiCommands.Delete, "chat.delete")
        });


        public async Task<dynamic> GetChatWebApiData(ApiCommands commandType, params KeyValuePair<string, string>[] queryParameters)
        {
            return await GetChatWebApiData(GetApiUri(commandType, queryParameters));
        }
        public async Task<dynamic> GetChatWebApiData(Uri uri)
        {
            var result = await new HttpClient().GetAsync(uri.ToString());
            return JObject.Parse(await result.Content.ReadAsStringAsync());
        }

        public async Task<dynamic> SendMessageAsync(string channel, string text)
        {
            return await GetChatWebApiData(GetApiUri(ApiCommands.Message, 
                new KeyValuePair<string,string>("channel", channel), 
                new KeyValuePair<string,string>("text", text)));
        }

        public async Task<dynamic> GetConnectionAsync()
        {
            return await GetChatWebApiData(GetApiUri(ApiCommands.Connect));
        }

        private Uri GetApiUri(ApiCommands commandType, params KeyValuePair<string, string>[] queryParameters)
        {
            var builder = new UriBuilder(string.Format("{0}{1}", urlBase, CommandDictionary[commandType]));

            var query = HttpUtility.ParseQueryString(builder.Query);
            query["token"] = _token;
            foreach (var param in queryParameters)
            {
                query[param.Key] = param.Value;
            }
            builder.Query = query.ToString();

            return builder.Uri;
        }
    }
}
