using System.Threading.Tasks;
using SentinelAI.Helpers;

namespace SentinelAI.Services
{
    public class IncidentInvestigator
    {
        public async Task<string> Investigate(string input)
        {
            var analyzer = new LocalAnalysisService(new LocalKnowledgeBase(ConfigurationLoader.Load().ReportsDirectory));
            return await Task.FromResult(analyzer.Analyze("Incident investigation", input));
        }
    }
}
