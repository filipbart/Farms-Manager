using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

namespace FarmsManager.Application.Queries.Accounting.InvoiceModuleAssignmentRules;

public record InvoiceModuleAssignmentRuleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Priority { get; init; }
    public ModuleType TargetModule { get; init; }
    public string TargetModuleName { get; init; } = string.Empty;
    public string[] IncludeKeywords { get; init; } = Array.Empty<string>();
    public string[] ExcludeKeywords { get; init; } = Array.Empty<string>();
    public Guid? TaxBusinessEntityId { get; init; }
    public string? TaxBusinessEntityName { get; init; }
    public Guid? FarmId { get; init; }
    public string? FarmName { get; init; }
    public KSeFInvoiceDirection? InvoiceDirection { get; init; }
    public string? InvoiceDirectionName { get; init; }
    public bool IsActive { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}
