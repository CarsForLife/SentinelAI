using System.IO;

namespace SentinelAI.Helpers
{
    public static class FileHelper
    {
        public static string Read(string path) => File.ReadAllText(path);
    }
}
