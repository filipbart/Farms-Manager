using System.ComponentModel;

namespace FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;

public enum ModuleType
{
    [Description("Brak przypisanego")] None,

    [Description("Pasze")] Feeds,

    [Description("Koszty produkcyjne")] ProductionExpenses,

    [Description("Gaz")] Gas,

    [Description("Sprzedaże (faktury sprzedażowe)")]
    Sales,

    [Description("Gospodarstwo rolne")] Farmstead,

    [Description("Inwestycje")] Investments,

    [Description("Nieruchomości")] RealEstate,

    [Description("Inne")] Other,
}