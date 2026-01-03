using System.Collections.Generic;

namespace SentinelAI.Models
{
    public class ScanResult
    {
        public bool Success { get; set; }
        public List<string> Findings { get; set; } = new();
    }
}
