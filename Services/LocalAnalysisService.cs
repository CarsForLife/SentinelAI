using System.Text.RegularExpressions;

namespace SentinelAI.Services;

public sealed class LocalAnalysisService
{
    private static readonly (string Name, string Technique, string Severity, string[] Terms)[] Rules =
    {
        ("Network download or remote execution", "T1105", "High", new[] { "Invoke-WebRequest", "curl ", "wget ", "DownloadString", "WebClient" }),
        ("Encoded or obfuscated payload", "T1027", "High", new[] { "base64", "FromBase64String", "-enc ", "EncodedCommand" }),
        ("Security tool tampering", "T1562.001", "Critical", new[] { "Add-MpPreference", "Set-MpPreference", "DisableAntiSpyware", "Defender" }),
        ("Credential access indicator", "T1555", "High", new[] { "password", "credential", "lsass", "sekurlsa" }),
        ("Persistence indicator", "T1547", "High", new[] { "RunOnce", "schtasks", "crontab", "startup" })
    };

    private readonly LocalKnowledgeBase _knowledge;

    public LocalAnalysisService(LocalKnowledgeBase knowledge)
    {
        _knowledge = knowledge;
    }

    public string Analyze(string operation, string input)
    {
        var findings = FindIndicators(input);
        var report = BuildReport(operation, findings, input);
        _knowledge.Record(operation, findings.Select(finding => finding.Description));
        return report;
    }

    public string Map(IEnumerable<string> indicators)
    {
        var input = string.Join(Environment.NewLine, indicators);
        return Analyze("MITRE mapping", input);
    }

    public string Classify(IEnumerable<string> indicators)
    {
        var findings = FindIndicators(string.Join(Environment.NewLine, indicators));
        var risk = findings.Count == 0 ? "Low" : findings.Any(finding => finding.Severity == "Critical") ? "Critical" : "High";
        var report = $"Local malware classification{Environment.NewLine}Risk level: {risk}{Environment.NewLine}Probable category: {(findings.Count == 0 ? "No known malware indicators" : "Suspicious activity")}{Environment.NewLine}{FormatFindings(findings)}{Environment.NewLine}{Recommendations(findings)}";
        _knowledge.Record("Malware classification", findings.Select(finding => finding.Description));
        return AddHistory(report);
    }

    private List<Finding> FindIndicators(string input)
    {
        return Rules
            .Where(rule => rule.Terms.Any(term => input.Contains(term, StringComparison.OrdinalIgnoreCase)))
            .Select(rule => new Finding(rule.Name, rule.Technique, rule.Severity, rule.Terms.First(term => input.Contains(term, StringComparison.OrdinalIgnoreCase))))
            .ToList();
    }

    private string BuildReport(string operation, List<Finding> findings, string input)
    {
        var operationInfo = SecurityOperationCatalog.Find(operation);
        var lines = new List<string>
        {
            $"Local {operation}",
            $"Category: {operationInfo.Category}",
            $"Purpose: {operationInfo.Purpose}",
            $"Input size: {input.Length:N0} characters",
            $"Risk level: {Risk(findings)}",
            "",
            "Findings:",
            FormatFindings(findings),
            "",
            Recommendations(findings)
        };
        return AddHistory(string.Join(Environment.NewLine, lines));
    }

    private string AddHistory(string report)
    {
        var previous = _knowledge.FrequentFindings();
        return previous.Count == 0
            ? report
            : $"{report}{Environment.NewLine}{Environment.NewLine}Prior local history:{Environment.NewLine}- {string.Join(Environment.NewLine + "- ", previous)}";
    }

    private static string FormatFindings(IEnumerable<Finding> findings)
    {
        var list = findings.ToList();
        return list.Count == 0
            ? "- No known indicators matched. Manual review is still recommended."
            : string.Join(Environment.NewLine, list.Select(finding => $"- {finding.Description} [{finding.Severity}] ({finding.Technique}); matched: {finding.Match}"));
    }

    private static string Recommendations(List<Finding> findings)
    {
        return findings.Count == 0
            ? "Recommendations: preserve the original evidence, validate the source, and review surrounding activity."
            : "Recommendations: isolate affected systems if activity is active, preserve evidence, review process and network timelines, and validate each finding with an analyst.";
    }

    private static string Risk(List<Finding> findings) => findings.Any(finding => finding.Severity == "Critical") ? "Critical" : findings.Any(finding => finding.Severity == "High") ? "High" : "Low";

    private sealed record Finding(string Description, string Technique, string Severity, string Match);
}