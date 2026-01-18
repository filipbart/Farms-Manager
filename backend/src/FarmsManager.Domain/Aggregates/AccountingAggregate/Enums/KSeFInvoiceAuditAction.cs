using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

public enum KSeFInvoiceAuditAction
{
    [Description("Zaakceptowano")] Accepted,

    [Description("Wstrzymano")] Held,

    [Description("Odrzucono")] Rejected,

    [Description("Przekazano do biura")] TransferredToOffice,

    [Description("Zmieniono status płatności")] PaymentStatusChanged,

    [Description("Przypisano pracownika")] EmployeeAssigned,

    [Description("Zmieniono moduł")] ModuleChanged,
}
