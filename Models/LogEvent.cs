namespace SentinelAI.Models
{
    public class LogEvent
    {
        public string Raw { get; set; }
        public override string ToString() => Raw;
    }
}
