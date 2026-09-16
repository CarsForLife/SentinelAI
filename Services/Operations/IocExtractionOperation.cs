namespace SentinelAI.Services.Operations;
public sealed class IocExtractionOperation : ISecurityOperation { public SecurityOperation Metadata => SecurityOperationCatalog.Find("IOC Extraction"); }