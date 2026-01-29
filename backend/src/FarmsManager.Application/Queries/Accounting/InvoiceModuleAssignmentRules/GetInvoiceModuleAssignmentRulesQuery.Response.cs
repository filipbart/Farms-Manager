using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

namespace FarmsManager.Application.Queries.Accounting.InvoiceModuleAssignmentRules;

public record InvoiceModuleAssignmentRuleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Priority { get; init; }
    public ModuleType TargetModule { get; init; }
    public string TargetModuleName { get; init; } = string.Empty;
    public string[] IncludeKeywords { get; init; } = [];
    public string[] ExcludeKeywords { get; init; } = [];
    public Guid? TaxBusinessEntityId { get; init; }
    public string TaxBusinessEntityName { get; init; } = string.Empty;
    public Guid? FarmId { get; init; }
    public string FarmName { get; init; } = string.Empty;
    public KSeFInvoiceDirection? InvoiceDirection { get; init; }
    public string InvoiceDirectionName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}
