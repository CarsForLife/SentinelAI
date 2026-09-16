using System.Reflection;

namespace SentinelAI.Services.Operations;

public static class SecurityOperationRegistry
{
    private static readonly IReadOnlyList<ISecurityOperation> Registered =
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(ISecurityOperation).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Select(type => (ISecurityOperation)Activator.CreateInstance(type)!)
            .OrderBy(operation => SecurityOperationCatalog.All.ToList().FindIndex(item => item.Name.Equals(operation.Metadata.Name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

    public static IReadOnlyList<ISecurityOperation> All => Registered;

    public static ISecurityOperation Find(string name)
    {
        return Registered.First(operation => operation.Metadata.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}