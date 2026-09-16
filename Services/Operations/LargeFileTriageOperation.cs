namespace SentinelAI.Services.Operations;
public sealed class LargeFileTriageOperation : ISecurityOperation { public SecurityOperation Metadata => SecurityOperationCatalog.Find("Large File Triage"); }