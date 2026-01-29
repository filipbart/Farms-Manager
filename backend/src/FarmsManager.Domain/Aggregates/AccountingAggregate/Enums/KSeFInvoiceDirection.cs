using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

/// <summary>
/// Kierunek faktury (sprzedaż/zakup)
/// </summary>
public enum KSeFInvoiceDirection
{
    [Description("Sprzedaż")] Sales,

    [Description("Zakup")] Purchase
}
