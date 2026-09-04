namespace SentinelAI.Models
{
    public sealed class AppSettings
    {
        public string LlmBaseUrl { get; set; } = "http://localhost:3000/v1/chat/completions";
        public string LlmModel { get; set; } = "gpt-4";
        public int LlmTimeoutSeconds { get; set; } = 30;
        public int MaxInputBytes { get; set; } = 10 * 1024 * 1024;
        public string ReportsDirectory { get; set; } = "Reports";
        public string YaraRulesPath { get; set; } = "Examples/yara_rules.yar";
    }
}
