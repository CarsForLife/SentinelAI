namespace SentinelAI.Services.Operations;

public sealed class OperationSelectionItem
{
    public OperationSelectionItem(ISecurityOperation operation, bool isSelected = false)
    {
        Operation = operation;
        IsSelected = isSelected;
    }

    public ISecurityOperation Operation { get; }
    public bool IsSelected { get; set; }
    public string Display => $"{Operation.Metadata.Name}  |  {Operation.Metadata.Category}";
}