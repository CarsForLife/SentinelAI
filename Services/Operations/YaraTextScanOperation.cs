namespace SentinelAI.Services.Operations;
public sealed class YaraTextScanOperation : ISecurityOperation { public SecurityOperation Metadata => SecurityOperationCatalog.Find("YARA Text Scan"); }