using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;
using SentinelAI.Helpers;

namespace SentinelAI.Services
{
    public class LlmClient
    {
        private readonly string _baseUrl;
        private readonly string _model;
        private readonly int _timeoutSeconds;

        public LlmClient()
        {
            var settings = ConfigurationLoader.Load();
            _baseUrl = settings.LlmBaseUrl;
            _model = settings.LlmModel;
            _timeoutSeconds = settings.LlmTimeoutSeconds;
        }

        public async Task<string> AskAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("A prompt is required.", nameof(prompt));

            var client = new RestClient(new RestClientOptions(_baseUrl)
            {
                MaxTimeout = _timeoutSeconds * 1000
            });
            var request = new RestRequest("", Method.Post);

            request.AddJsonBody(new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            });

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
                throw new InvalidOperationException($"LLM request failed ({(int)response.StatusCode}): {response.ErrorMessage ?? response.StatusDescription}");
            if (string.IsNullOrWhiteSpace(response.Content))
                throw new InvalidOperationException("LLM returned an empty response.");

            var json = JObject.Parse(response.Content);
            var content = json["choices"]?[0]?["message"]?["content"]?.ToString();
            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("LLM response did not contain choices[0].message.content.");

            return content;
        }
    }
}
