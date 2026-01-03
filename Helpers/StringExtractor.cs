using System.Collections.Generic;
using System.Linq;

namespace SentinelAI.Helpers
{
    public static class StringExtractor
    {
        public static List<string> ExtractStrings(string input)
        {
            return input.Split('\n').ToList();
        }
    }
}
