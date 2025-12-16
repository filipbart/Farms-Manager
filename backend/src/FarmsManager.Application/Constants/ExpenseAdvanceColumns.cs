namespace FarmsManager.Application.Constants;

/// <summary>
/// Stałe definiujące dostępne kolumny w ewidencji zaliczek
/// </summary>
public static class ExpenseAdvanceColumns
{
    public const string Date = "date";
    public const string Type = "type";
    public const string Name = "name";
    public const string Amount = "amount";
    public const string CategoryName = "categoryName";
    public const string Comment = "comment";
    public const string FilePath = "filePath";
    public const string DateCreatedUtc = "dateCreatedUtc";

    /// <summary>
    /// Wszystkie dostępne kolumny
    /// </summary>
    public static readonly string[] AllColumns =
    [
        Date,
        Type,
        Name,
        Amount,
        CategoryName,
        Comment,
        FilePath,
        DateCreatedUtc
    ];

    /// <summary>
    /// Domyślne kolumny dla nowych użytkowników (podstawowe)
    /// </summary>
    public static readonly string[] DefaultColumns =
    [
        Date,
        Type,
        Name,
        Amount,
        CategoryName
    ];

    /// <summary>
    /// Opisy kolumn (do wyświetlania w UI)
    /// </summary>
    public static readonly Dictionary<string, string> ColumnDescriptions = new()
    {
        { Date, "Data" },
        { Type, "Typ" },
        { Name, "Nazwa" },
        { Amount, "Kwota [zł]" },
        { CategoryName, "Kategoria" },
        { Comment, "Komentarz" },
        { FilePath, "Plik" },
        { DateCreatedUtc, "Data utworzenia wpisu" }
    };
}
