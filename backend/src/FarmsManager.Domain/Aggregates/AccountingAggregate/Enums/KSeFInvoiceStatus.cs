using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

public enum KSeFInvoiceStatus
{
    [Description("Nowa")] New,

    [Description("Odrzucona")] Rejected,

    [Description("Zaakceptowana")] Accepted,

    [Description("Przekazana do biura")] SentToOffice,
}