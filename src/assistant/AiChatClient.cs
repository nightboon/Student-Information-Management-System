using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using src.services;

namespace src.assistant
{
    /// <summary>
    /// Thin HTTP client for an OpenAI-compatible Chat Completions endpoint.
    /// Knows the wire protocol only; it has no knowledge of the portal's data.
    /// Endpoint, key and model ("Ai:BaseUrl", "Ai:ApiKey", "Ai:Model") come from
    /// AiConsoleService, which reads ai.config directly so AI Console edits
    /// apply immediately without an app restart.
    /// </summary>
    public static class AiChatClient
    {
        private static readonly JavaScriptSerializer Json =
            new JavaScriptSerializer { MaxJsonLength = 10_000_000 };

        /// <summary>
        /// Sends one round of messages plus the available tools and returns the
        /// assistant message object (which may contain "tool_calls"). Throws on
        /// transport/HTTP errors; the caller is expected to handle them.
        /// </summary>
        public static Dictionary<string, object> Send(List<object> messages, List<object> tools)
        {
            string baseUrl = AiConsoleService.GetSetting("Ai:BaseUrl").TrimEnd('/');
            string apiKey = AiConsoleService.GetSetting("Ai:ApiKey");
            string model = AiConsoleService.GetSetting("Ai:Model");

            var payload = new Dictionary<string, object>
            {
                ["model"] = model,
                ["messages"] = messages,
                ["tools"] = tools,
                ["tool_choice"] = "auto"
            };

            string requestBody = Json.Serialize(payload);

            // Some compatible endpoints still default to older TLS; force 1.2.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "/chat/completions");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers["Authorization"] = "Bearer " + apiKey;
            request.Timeout = 60000;

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(requestBody);
            }

            string responseText;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                responseText = reader.ReadToEnd();
            }

            var root = (Dictionary<string, object>)Json.DeserializeObject(responseText);
            var choices = (object[])root["choices"];
            var first = (Dictionary<string, object>)choices[0];
            return (Dictionary<string, object>)first["message"];
        }
    }
}
