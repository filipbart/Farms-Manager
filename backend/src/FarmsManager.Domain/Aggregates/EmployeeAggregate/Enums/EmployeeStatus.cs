using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;

public enum EmployeeStatus
{
    [Description("Aktywny")]
    Active,
    
    [Description("Nieaktywny")]
    Inactive
}