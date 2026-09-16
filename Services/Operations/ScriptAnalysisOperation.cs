namespace SentinelAI.Services.Operations;
public sealed class ScriptAnalysisOperation : ISecurityOperation { public SecurityOperation Metadata => SecurityOperationCatalog.Find("Script Analysis"); }