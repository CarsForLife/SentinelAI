namespace SentinelAI.Services.Operations;
public sealed class MitreMappingOperation : ISecurityOperation { public SecurityOperation Metadata => SecurityOperationCatalog.Find("MITRE Mapping"); }