namespace SentinelAI.Models
{
    public class LogEvent
    {
        public string Raw { get; set; } = string.Empty;
        public override string ToString() => Raw;
    }
}
