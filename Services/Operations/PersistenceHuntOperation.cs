namespace SentinelAI.Services.Operations;
public sealed class PersistenceHuntOperation : ISecurityOperation { public SecurityOperation Metadata => SecurityOperationCatalog.Find("Persistence Hunt"); }