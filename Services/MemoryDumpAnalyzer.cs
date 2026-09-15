using System.Linq;
using System.Threading.Tasks;
using SentinelAI.Helpers;

namespace SentinelAI.Services
{
    public class MemoryDumpAnalyzer
    {
        public async Task<string> Summarize(string[] strings)
        {
            var extract = string.Join("\n", strings.Take(200));
            var analyzer = new LocalAnalysisService(new LocalKnowledgeBase(ConfigurationLoader.Load().ReportsDirectory));
            return await Task.FromResult(analyzer.Analyze("Memory dump analysis", extract));
        }
    }
}
