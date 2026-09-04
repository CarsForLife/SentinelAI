using System.Collections.Generic;
using System.IO;
using SentinelAI.Helpers;
using SentinelAI.Models;

namespace SentinelAI.Services
{
    public class LogAnalyzer
    {
        public List<LogEvent> ParseLogFile(string path, int maxBytes = 10 * 1024 * 1024)
        {
            var lines = FileHelper.ReadLines(path, maxBytes);
            var events = new List<LogEvent>();

            foreach (var line in lines)
            {
                events.Add(new LogEvent { Raw = line });
            }

            return events;
        }
    }
}
