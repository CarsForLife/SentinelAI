using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace SentinelAI.Services
{
    public class LlmClient
    {
        private readonly string _baseUrl = "http://localhost:3000/v1/chat/completions";

        public async Task<string> AskAsync(string prompt)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("", Method.Post);

            request.AddJsonBody(new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            });

            var response = await client.ExecuteAsync(request);
            var json = JObject.Parse(response.Content);

            return json["choices"][0]["message"]["content"].ToString();
        }
    }
}
