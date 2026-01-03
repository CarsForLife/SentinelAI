using System.Collections.Generic;
using System.IO;
using SentinelAI.Models;

namespace SentinelAI.Services
{
    public class LogAnalyzer
    {
        public List<LogEvent> ParseLogFile(string path)
        {
            var lines = File.ReadAllLines(path);
            var events = new List<LogEvent>();

            foreach (var line in lines)
            {
                events.Add(new LogEvent { Raw = line });
            }

            return events;
        }
    }
}
