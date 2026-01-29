using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

public enum KSeFInvoicePaymentType
{
    [Description("Gotówka")] Cash,

    [Description("Przelew")] BankTransfer
}