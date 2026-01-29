using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;

public class KSeFInvoiceAuditLogEntity : Entity
{
    protected KSeFInvoiceAuditLogEntity()
    {
    }

    public static KSeFInvoiceAuditLogEntity CreateNew(
        Guid invoiceId,
        KSeFInvoiceAuditAction action,
        KSeFInvoiceStatus? previousStatus,
        KSeFInvoiceStatus? newStatus,
        Guid userId,
        string userName,
        string comment = null)
    {
        return new KSeFInvoiceAuditLogEntity
        {
            InvoiceId = invoiceId,
            Action = action,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            UserId = userId,
            UserName = userName,
            Comment = comment,
            CreatedBy = userId
        };
    }

    /// <summary>
    /// Identyfikator faktury, której dotyczy log
    /// </summary>
    public Guid InvoiceId { get; init; }

    /// <summary>
    /// Faktura, której dotyczy log
    /// </summary>
    public virtual KSeFInvoiceEntity Invoice { get; init; }

    /// <summary>
    /// Typ akcji wykonanej na fakturze
    /// </summary>
    public KSeFInvoiceAuditAction Action { get; init; }

    /// <summary>
    /// Poprzedni status faktury (przed zmianą)
    /// </summary>
    public KSeFInvoiceStatus? PreviousStatus { get; init; }

    /// <summary>
    /// Nowy status faktury (po zmianie)
    /// </summary>
    public KSeFInvoiceStatus? NewStatus { get; init; }

    /// <summary>
    /// Identyfikator użytkownika, który wykonał akcję
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Użytkownik, który wykonał akcję
    /// </summary>
    public virtual UserEntity User { get; init; }

    /// <summary>
    /// Nazwa użytkownika w momencie wykonania akcji (dla celów historycznych)
    /// </summary>
    public string UserName { get; init; }

    /// <summary>
    /// Dodatkowy komentarz do akcji
    /// </summary>
    public string Comment { get; init; }
}
