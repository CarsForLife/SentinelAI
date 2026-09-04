using System;
using System.IO;
using System.Text;

namespace SentinelAI.Helpers
{
    public static class FileHelper
    {
        public static string Read(string path, int maxBytes = 10 * 1024 * 1024)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("A file path is required.", nameof(path));
            if (!File.Exists(path))
                throw new FileNotFoundException("The input file was not found.", path);
            if (new FileInfo(path).Length > maxBytes)
                throw new IOException($"The input file exceeds the {maxBytes:N0}-byte limit.");

            return File.ReadAllText(path, Encoding.UTF8);
        }

        public static string[] ReadLines(string path, int maxBytes = 10 * 1024 * 1024) =>
            Read(path, maxBytes).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        public static void WriteReport(string directory, string name, string content)
        {
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, name), content, Encoding.UTF8);
        }
    }
}
