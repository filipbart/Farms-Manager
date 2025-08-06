using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Enums;

public enum ExpenseAdvanceCategoryType
{
    [Description("Przychód")]
    Income,
    
    [Description("Wydatek")]
    Expense
}