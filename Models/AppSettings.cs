namespace SentinelAI.Models
{
    public sealed class AppSettings
    {
        public int MaxInputBytes { get; set; } = 10 * 1024 * 1024;
        public string ReportsDirectory { get; set; } = "Reports";
        public string YaraRulesPath { get; set; } = "Examples/yara_rules.yar";
    }
}
