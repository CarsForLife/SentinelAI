namespace SentinelAI.Services.Operations;
public sealed class LogAnalysisOperation : ISecurityOperation { public SecurityOperation Metadata => SecurityOperationCatalog.Find("Log Analysis"); }