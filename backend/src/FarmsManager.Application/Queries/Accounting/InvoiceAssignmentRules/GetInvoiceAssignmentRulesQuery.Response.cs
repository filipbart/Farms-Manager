namespace FarmsManager.Application.Queries.Accounting.InvoiceAssignmentRules;

public record InvoiceAssignmentRuleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; }
    public int Priority { get; init; }
    public Guid AssignedUserId { get; init; }
    public string AssignedUserName { get; init; } = string.Empty;
    public string[] IncludeKeywords { get; init; } = [];
    public string[] ExcludeKeywords { get; init; } = [];
    public Guid? TaxBusinessEntityId { get; init; }
    public string TaxBusinessEntityName { get; init; }
    public Guid? FarmId { get; init; }
    public string FarmName { get; init; }
    public bool IsActive { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}
