using System.Text.Json;

namespace SentinelAI.Services;

public sealed class LocalKnowledgeBase
{
    private readonly string _path;
    private readonly List<KnowledgeEntry> _entries;

    public LocalKnowledgeBase(string reportsDirectory)
    {
        Directory.CreateDirectory(reportsDirectory);
        _path = Path.Combine(reportsDirectory, "user_history.json");
        _entries = Load();
    }

    public void Record(string operation, IEnumerable<string> findings)
    {
        var normalized = findings
            .Where(finding => !string.IsNullOrWhiteSpace(finding))
            .Select(finding => finding.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        _entries.Add(new KnowledgeEntry(DateTimeOffset.UtcNow, operation, normalized));
        File.WriteAllText(_path, JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true }));
    }

    public IReadOnlyList<string> FrequentFindings(int limit = 5)
    {
        return _entries
            .SelectMany(entry => entry.Findings)
            .GroupBy(finding => finding, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key)
            .Take(limit)
            .Select(group => $"{group.Key} ({group.Count()} previous occurrence{(group.Count() == 1 ? "" : "s")})")
            .ToList();
    }

    private List<KnowledgeEntry> Load()
    {
        if (!File.Exists(_path))
            return new List<KnowledgeEntry>();

        try
        {
            return JsonSerializer.Deserialize<List<KnowledgeEntry>>(File.ReadAllText(_path)) ?? new List<KnowledgeEntry>();
        }
        catch (JsonException)
        {
            return new List<KnowledgeEntry>();
        }
    }

    private sealed record KnowledgeEntry(DateTimeOffset Timestamp, string Operation, List<string> Findings);
}