using System.Linq;
using System.Threading.Tasks;

namespace SentinelAI.Services
{
    public class MemoryDumpAnalyzer
    {
        private readonly LlmClient llm = new();

        public async Task<string> Summarize(string[] strings)
        {
            var extract = string.Join("\n", strings.Take(200));
            return await llm.AskAsync($"Summarize potential threats from memory strings:\n{extract}");
        }
    }
}
