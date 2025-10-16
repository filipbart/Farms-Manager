using FarmsManager.Application.Models.Notifications;

namespace FarmsManager.Application.Queries.Dashboard;

public record GetDashboardDataQueryResponse
{
    //Statystki
    public DashboardStats Stats { get; set; }

    //Wykresy liniowe
    public DashboardFcrChart FcrChart { get; set; }
    public DashboardGasConsumptionChart GasConsumptionChart { get; set; }
    public DashboardEwwChart EwwChart { get; set; }
    public DashboardFlockLossChart FlockLossChart { get; set; }

    //Statusy ferm
    public DashboardChickenHousesStatus ChickenHousesStatus { get; set; }

    //Wykres kołowy
    public DashboardExpensesPieChart ExpensesPieChart { get; set; }

    //Powiadomienia
    public List<DashboardNotificationItem> Notifications { get; set; }
}

public record DashboardStats
{
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal Income => Revenue - Expenses;
    public decimal VatFromExpenses { get; set; }
    public decimal IncomePerKg { get; set; }
    public decimal IncomePerSqm { get; set; }
    public decimal AvgFeedPrice { get; set; }
}

public record DashboardHenhouseStatus
{
    public string Name { get; set; }
    public int ChickenCount { get; set; } //EndCycleBirdBalance
    public DateOnly? InsertionDate { get; set; }
}

public record DashboardFarmStatus
{
    public string Name { get; set; }
    public int HenhousesCount => Henhouses?.Count ?? 0;
    public int ChickenCount => Henhouses?.Sum(t => t.ChickenCount) ?? 0;

    public List<DashboardHenhouseStatus> Henhouses { get; set; } = [];
}

public record DashboardChickenHousesStatus
{
    public List<DashboardFarmStatus> Farms { get; set; }
    public int TotalHenhousesCount => Farms.Sum(t => t.HenhousesCount);
    public int TotalChickenCount => Farms.Sum(t => t.ChickenCount);
}

// Generyczne, reużywalne rekordy dla danych wykresów liniowych
public record ChartSeries
{
    public Guid FarmId { get; set; }
    public string FarmName { get; set; }
    public List<ChartDataPoint> Data { get; set; } = [];
}

public record ChartDataPoint
{
    public string X { get; set; }
    public decimal? Y { get; set; }
}

// Struktury dla poszczególnych wykresów liniowych
public record DashboardFcrChart
{
    public List<ChartSeries> Series { get; set; } = [];
}

public record DashboardGasConsumptionChart
{
    public List<ChartSeries> Series { get; set; } = [];
}

public record DashboardEwwChart
{
    public List<ChartSeries> Series { get; set; } = [];
}

public record DashboardFlockLossChart
{
    public List<ChartSeries> Series { get; set; } = [];
}

// Rekordy dla wykresu kołowego wydatków
public record DashboardExpensesPieChart
{
    public List<ExpensesPieChartDataPoint> Data { get; set; } = [];
}

public record ExpensesPieChartDataPoint
{
    public string Id { get; set; }
    public string Label { get; set; }
    public decimal Value { get; set; }
}

public enum NotificationType
{
    SaleInvoice,
    FeedInvoice,
    EmployeeContract,
    EmployeeReminder
}

public record DashboardNotificationItem
{
    /// <summary>
    /// Wygenerowany opis powiadomienia, np. "Koniec umowy dla Jan Kowalski: 20.08.2025"
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Data końcowa zdarzenia
    /// </summary>
    public DateOnly DueDate { get; set; }

    /// <summary>
    /// Priorytet zdarzenia (Wysoki, Średni, Niski)
    /// </summary>
    public NotificationPriority Priority { get; set; }

    /// <summary>
    /// Typ powiadomienia, do ewentualnego użycia na froncie (np. do ikonki)
    /// </summary>
    public NotificationType Type { get; set; }
    
    /// <summary>
    /// ID źródłowej encji (faktury, pracownika, przypomnienia) do nawigacji.
    /// </summary>
    public Guid? SourceId { get; set; }
}