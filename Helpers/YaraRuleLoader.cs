using System.IO;

namespace SentinelAI.Helpers
{
    public static class YaraRuleLoader
    {
        public static string LoadDefaultRules()
        {
            return File.ReadAllText("Examples/yara_rules.yar");
        }
    }
}
