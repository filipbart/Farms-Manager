using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

/// <summary>
/// Encja reprezentująca powiązanie między fakturami
/// </summary>
public class KSeFInvoiceRelationEntity : Entity
{
    protected KSeFInvoiceRelationEntity()
    {
    }

    /// <summary>
    /// Tworzy nowe powiązanie między fakturami
    /// </summary>
    public static KSeFInvoiceRelationEntity CreateNew(
        Guid sourceInvoiceId,
        Guid targetInvoiceId,
        InvoiceRelationType relationType,
        Guid? userId = null)
    {
        return new KSeFInvoiceRelationEntity
        {
            SourceInvoiceId = sourceInvoiceId,
            TargetInvoiceId = targetInvoiceId,
            RelationType = relationType,
            CreatedBy = userId
        };
    }

    /// <summary>
    /// Identyfikator faktury źródłowej (np. korekta, zaliczka)
    /// </summary>
    public Guid SourceInvoiceId { get; init; }

    /// <summary>
    /// Faktura źródłowa
    /// </summary>
    public virtual KSeFInvoiceEntity SourceInvoice { get; init; }

    /// <summary>
    /// Identyfikator faktury docelowej (np. faktura pierwotna, końcowa)
    /// </summary>
    public Guid TargetInvoiceId { get; init; }

    /// <summary>
    /// Faktura docelowa
    /// </summary>
    public virtual KSeFInvoiceEntity TargetInvoice { get; init; }

    /// <summary>
    /// Typ powiązania
    /// </summary>
    public InvoiceRelationType RelationType { get; init; }
}
