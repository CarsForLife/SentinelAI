namespace SentinelAI.Services.Operations;
public sealed class UrlExtractionOperation : ISecurityOperation { public SecurityOperation Metadata => SecurityOperationCatalog.Find("URL Extraction"); }