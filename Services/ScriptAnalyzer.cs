using System.Collections.Generic;

namespace SentinelAI.Services
{
    public class ScriptAnalyzer
    {
        public List<string> Scan(string script)
        {
            var findings = new List<string>();

            if (script.Contains("Invoke-WebRequest"))
                findings.Add("Detected: Network call");

            if (script.Contains("base64"))
                findings.Add("Detected: Base64 encoded payload");

            if (script.Contains("Add-MpPreference"))
                findings.Add("Detected: Attempt to disable Defender");

            return findings;
        }
    }
}
