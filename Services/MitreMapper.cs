using System.Threading.Tasks;
using SentinelAI.Helpers;

namespace SentinelAI.Services
{
    public class MitreMapper
    {
        public async Task<string> Map(string[] indicators)
        {
            var analyzer = new LocalAnalysisService(new LocalKnowledgeBase(ConfigurationLoader.Load().ReportsDirectory));
            return await Task.FromResult(analyzer.Map(indicators));
        }
    }
}
