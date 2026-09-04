using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

// This is a deterministic fallback; it does not execute YARA syntax.

namespace SentinelAI.Services
{
    public class YaraScanner
    {
        public List<string> ScanFile(string file, int maxBytes = 10 * 1024 * 1024)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentException("A file path is required.", nameof(file));
            if (!File.Exists(file))
                throw new FileNotFoundException("The file to scan was not found.", file);

            if (new FileInfo(file).Length > maxBytes)
                throw new IOException($"The file exceeds the {maxBytes:N0}-byte limit.");

            var content = File.ReadAllText(file, Encoding.UTF8);
            var findings = new List<string>();
            if (content.Contains("Invoke-WebRequest", StringComparison.OrdinalIgnoreCase))
                findings.Add("Deterministic signature: network download command");
            if (content.Contains("FromBase64String", StringComparison.OrdinalIgnoreCase))
                findings.Add("Deterministic signature: Base64 decoding");
            if (content.Contains("Add-MpPreference", StringComparison.OrdinalIgnoreCase))
                findings.Add("Deterministic signature: Defender exclusion change");
            return findings;
        }
    }
}
