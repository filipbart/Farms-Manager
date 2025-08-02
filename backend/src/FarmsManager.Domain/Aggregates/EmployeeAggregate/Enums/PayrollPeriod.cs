using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.EmployeeAggregate.Enums;

public enum PayrollPeriod
{
    [Description("Styczeń")]
    January,

    [Description("Luty")]
    February,

    [Description("Marzec")]
    March,

    [Description("Kwiecień")]
    April,

    [Description("Maj")]
    May,

    [Description("Czerwiec")]
    June,

    [Description("Lipiec")]
    July,

    [Description("Sierpień")]
    August,

    [Description("Wrzesień")]
    September,

    [Description("Październik")]
    October,

    [Description("Listopad")]
    November,

    [Description("Grudzień")]
    December
}