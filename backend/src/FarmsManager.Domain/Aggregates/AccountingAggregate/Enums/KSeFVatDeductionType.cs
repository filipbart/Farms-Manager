using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

public enum KSeFVatDeductionType
{
    [Description("100%")] Full,
    [Description("50%")] Half,
    [Description("Brak")] None
}