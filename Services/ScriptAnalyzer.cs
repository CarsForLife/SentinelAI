using System.Collections.Generic;

namespace SentinelAI.Services
{
    public class ScriptAnalyzer
    {
        public List<string> Scan(string script)
        {
            var findings = new List<string>();

            if (script.Contains("Invoke-WebRequest", StringComparison.OrdinalIgnoreCase))
                findings.Add("Detected: Network call");

            if (script.Contains("base64", StringComparison.OrdinalIgnoreCase))
                findings.Add("Detected: Base64 encoded payload");

            if (script.Contains("Add-MpPreference", StringComparison.OrdinalIgnoreCase))
                findings.Add("Detected: Attempt to disable Defender");

            return findings;
        }
    }
}
