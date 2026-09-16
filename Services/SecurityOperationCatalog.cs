namespace SentinelAI.Services;

public sealed record SecurityOperation(string Name, string Category, string Purpose);

public static class SecurityOperationCatalog
{
    public static IReadOnlyList<SecurityOperation> All { get; } = new[]
    {
        Op("Log Analysis", "Triage", "Parse log evidence and identify known threat indicators."),
        Op("Script Analysis", "Endpoint", "Review script content for suspicious execution behavior."),
        Op("YARA Text Scan", "Detection", "Apply local text signatures to a file without executing YARA syntax."),
        Op("Memory Strings", "Forensics", "Triage extracted memory strings for suspicious indicators."),
        Op("MITRE Mapping", "Frameworks", "Map recognized indicators to ATT&CK technique IDs."),
        Op("Malware Classification", "Malware", "Classify local evidence into a deterministic risk category."),
        Op("Incident Investigation", "Response", "Produce an evidence summary and response recommendations."),
        Op("Evidence Integrity Check", "Forensics", "Record file size, encoding, and SHA-256 evidence identity."),
        Op("File Metadata Review", "Forensics", "Review file names, extensions, timestamps, and basic metadata."),
        Op("Hash Comparison", "Forensics", "Compare supplied hashes and identify repeated evidence identities."),
        Op("IOC Extraction", "Threat Intelligence", "Extract candidate indicators of compromise from text."),
        Op("IOC Normalization", "Threat Intelligence", "Normalize indicators for consistent analyst review."),
        Op("Domain Extraction", "Threat Intelligence", "Find domain-like values for controlled investigation."),
        Op("IP Address Extraction", "Threat Intelligence", "Find IPv4 and IPv6-like values in evidence."),
        Op("URL Extraction", "Threat Intelligence", "Find URL-like values without contacting them."),
        Op("Email Indicator Extraction", "Threat Intelligence", "Find email addresses and mail-routing clues."),
        Op("Windows Event Triage", "Endpoint", "Triage Windows event text for authentication and execution clues."),
        Op("PowerShell Threat Hunt", "Endpoint", "Review PowerShell text for download, encoding, and bypass behavior."),
        Op("Command Line Triage", "Endpoint", "Review process command lines for suspicious arguments."),
        Op("Persistence Hunt", "Endpoint", "Look for recurring persistence-related indicators."),
        Op("Credential Access Hunt", "Endpoint", "Look for credential theft and secret-access indicators."),
        Op("Defense Evasion Hunt", "Endpoint", "Look for security-control tampering and obfuscation."),
        Op("Lateral Movement Hunt", "Network", "Look for remote execution and lateral movement clues."),
        Op("C2 Indicator Hunt", "Network", "Look for command-and-control and beaconing clues."),
        Op("Exfiltration Indicator Hunt", "Network", "Look for bulk transfer and data-staging clues."),
        Op("Ransomware Precursor Hunt", "Response", "Look for precursor behaviors associated with ransomware response."),
        Op("Phishing Artifact Triage", "Email", "Review message text and artifacts for phishing signals."),
        Op("Web Shell Indicator Scan", "Application", "Look for web-shell names, commands, and execution clues."),
        Op("Secrets Exposure Scan", "Application", "Find password, token, key, and credential-like text."),
        Op("Configuration Drift Review", "Hardening", "Review configuration text for risky or unexpected changes."),
        Op("Access Log Review", "Triage", "Review access records for unusual paths and patterns."),
        Op("Authentication Failure Review", "Identity", "Review failed authentication patterns and account signals."),
        Op("Privilege Escalation Hunt", "Identity", "Look for privilege-change and escalation clues."),
        Op("Scheduled Task Review", "Persistence", "Review scheduled task text for suspicious execution."),
        Op("Service Persistence Review", "Persistence", "Review service installation and startup clues."),
        Op("Registry Persistence Review", "Persistence", "Review registry-run and policy persistence clues."),
        Op("Startup Folder Review", "Persistence", "Review startup locations and autorun clues."),
        Op("Network Connection Review", "Network", "Review connection text for remote endpoints and unusual services."),
        Op("Process String Review", "Forensics", "Review extracted process strings for threat indicators."),
        Op("DLL Hijack Indicator Scan", "Endpoint", "Look for DLL search-order and loading clues."),
        Op("Living-off-the-Land Review", "Endpoint", "Look for trusted tools used in suspicious contexts."),
        Op("Encoded Content Review", "Detection", "Find base64, encoded command, and obfuscation indicators."),
        Op("Archive Artifact Review", "Forensics", "Review archive names and extraction clues."),
        Op("Suspicious File Name Review", "Triage", "Review names and extensions commonly associated with abuse."),
        Op("Baseline Comparison", "Hardening", "Compare current evidence against supplied baseline text."),
        Op("Case Timeline Builder", "Response", "Order dated evidence lines for analyst timeline review."),
        Op("Severity Prioritization", "Triage", "Prioritize findings using deterministic severity rules."),
        Op("ATT&CK Coverage Summary", "Frameworks", "Summarize recognized technique coverage in evidence."),
        Op("NIST CSF Evidence Notes", "Governance", "Organize findings as analyst notes against NIST CSF themes."),
        Op("Analyst Handoff Package", "Response", "Prepare concise findings and evidence notes for handoff."),
        Op("Incident Containment Checklist", "Response", "Generate local containment actions based on matched indicators."),
        Op("Recovery Verification Checklist", "Response", "Generate post-incident verification prompts."),
        Op("Report Quality Check", "Governance", "Check whether evidence reports contain identity and analyst context."),
        Op("History Trend Summary", "Knowledge", "Summarize recurring findings from local history."),
        Op("Duplicate Finding Detection", "Knowledge", "Identify repeated indicator descriptions in supplied evidence."),
        Op("Evidence Redaction Preview", "Privacy", "Highlight credential-like terms for manual redaction review."),
        Op("Input Encoding Check", "Forensics", "Check text for replacement characters and encoding anomalies."),
        Op("Large File Triage", "Forensics", "Perform bounded local triage on large evidence files."),
        Op("Rule Coverage Review", "Detection", "Show which local detection families matched evidence."),
        Op("Local Playbook Lookup", "Knowledge", "Provide the relevant local defensive workflow context."),
        Op("Security Training Drill", "Training", "Turn supplied evidence into an analyst practice exercise."),
        Op("Defensive Recommendation Review", "Hardening", "Review findings and produce analyst-verifiable defensive next steps.")
    };

    public static SecurityOperation Find(string name)
    {
        return All.FirstOrDefault(operation => operation.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? Op(name, "Custom", "Run the local deterministic analysis rules against supplied evidence.");
    }

    private static SecurityOperation Op(string name, string category, string purpose) => new(name, category, purpose);
}
