using System.Threading.Tasks;

namespace SentinelAI.Services
{
    public class MitreMapper
    {
        private readonly LlmClient llm = new();

        public async Task<string> Map(string[] indicators)
        {
            string prompt = $@"
Map the following indicators to MITRE ATT&CK:

{string.Join("\n", indicators)}

Return:
- Technique ID
- Name
- Reasoning
- Severity
";

            return await llm.AskAsync(prompt);
        }
    }
}
