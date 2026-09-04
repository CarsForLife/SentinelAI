using System.IO;

namespace SentinelAI.Helpers
{
    public static class YaraRuleLoader
    {
        public static string LoadDefaultRules(string path = "Examples/yara_rules.yar")
        {
            if (!File.Exists(path))
                return string.Empty;
            return File.ReadAllText(path);
        }
    }
}
