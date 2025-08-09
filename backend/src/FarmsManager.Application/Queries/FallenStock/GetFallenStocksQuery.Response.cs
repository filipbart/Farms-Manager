namespace FarmsManager.Application.Queries.FallenStock;

/// <summary>
/// Główny model widoku dla tabeli sztuk padłych,
/// który jest wysyłany z API do frontendu.
/// </summary>
public record FallenStockTableViewModel
{
    /// <summary>
    /// Lista dynamicznie generowanych kolumn (np. K1, K2... K8, Pozostało).
    /// Frontend użyje tego do zbudowania definicji kolumn w DataGrid.
    /// </summary>
    public List<ColumnHeaderModel> HenhouseColumns { get; set; } = [];

    /// <summary>
    /// Główne wiersze danych (daty wstawień).
    /// Frontend przekaże to do propa `rows` w DataGrid.
    /// </summary>
    public List<TableRowModel> InsertionRows { get; set; } = [];

    /// <summary>
    /// Wiersze podsumowujące (Ubiórka, Sprzedaż, Stan stada).
    /// Frontend przekaże to do propa `pinnedRows` w DataGrid.
    /// </summary>
    public List<TableRowModel> SummaryRows { get; set; } = [];

    /// <summary>
    /// Ostateczna, zsumowana wartość "Pozostało" ze wszystkich kurników w wierszu "Stan stada".
    /// </summary>
    public int GrandTotal { get; set; }
}

/// <summary>
/// Reprezentuje pojedynczą, dynamiczną kolumnę (kurnik).
/// </summary>
public record ColumnHeaderModel
{
    /// <summary>
    /// ID kurnika (Guid przekonwertowany na string), używane jako `field` w DataGrid.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Nazwa wyświetlana w nagłówku kolumny (np. "K1").
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// Reprezentuje pojedynczy wiersz w finalnej tabeli.
/// Może to być zarówno wiersz wstawienia, jak i wiersz podsumowania.
/// </summary>
public record TableRowModel
{
    /// <summary>
    /// Unikalny identyfikator wiersza dla klucza w React (np. "insertion_2025-06-16" lub "summary_sales").
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Tytuł wiersza wyświetlany w pierwszej kolumnie (np. "16.06.2025" lub "Sprzedaż").
    /// </summary>
    public string RowTitle { get; set; }
    
    /// <summary>
    /// Opisowy typ zgłoszenia (np. "Zgłoszenie upadków"), wyświetlany pod datą.
    /// Będzie `null` dla wierszy, które nie są zgłoszeniami (np. podsumowania).
    /// </summary>
    public string TypeDesc { get; set; }

    /// <summary>
    /// Słownik przechowujący wartości dla każdego kurnika.
    /// Klucz to ID kurnika (string), wartość to liczba sztuk.
    /// Użycie słownika idealnie pasuje do dynamicznej liczby kolumn.
    /// </summary>
    public Dictionary<string, int?> HenhouseValues { get; set; } = new();

    /// <summary>
    /// Wartość dla kolumny "Pozostało", jeśli dotyczy danego wiersza.
    /// </summary>
    public int? Remaining { get; set; }
    
    /// <summary>
    /// Czy wysłane do IRZplus - blokuje przycisk usuwania
    /// </summary>
    public bool IsSentToIrz { get; set; }
}

public record GetFallenStocksQueryResponse : FallenStockTableViewModel;