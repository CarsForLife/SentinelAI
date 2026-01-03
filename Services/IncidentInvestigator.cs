using System.Threading.Tasks;

namespace SentinelAI.Services
{
    public class IncidentInvestigator
    {
        private readonly LlmClient llm = new();

        public async Task<string> Investigate(string input)
        {
            var prompt = $@"
Investigate this input. Create:

- Timeline
- Attack intent
- Indicators of compromise
- MITRE mapping
- Severity
- Recommendations

Input:
{input}
";

            return await llm.AskAsync(prompt);
        }
    }
}
