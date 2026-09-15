using System;
using System.IO;
using System.Text.Json;
using SentinelAI.Models;

namespace SentinelAI.Helpers
{
    public static class ConfigurationLoader
    {
        public static AppSettings Load()
        {
            var settings = new AppSettings();
            var path = Path.Combine(AppContext.BaseDirectory, "Config", "settings.json");

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                settings = JsonSerializer.Deserialize<AppSettings>(json) ?? settings;
            }

            if (settings.MaxInputBytes < 1)
                throw new InvalidOperationException("Config/settings.json contains an invalid input limit.");

            return settings;
        }
    }
}
