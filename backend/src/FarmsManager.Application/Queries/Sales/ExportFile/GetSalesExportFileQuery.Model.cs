using FarmsManager.Shared.Attributes;

namespace FarmsManager.Application.Queries.Sales.ExportFile;

public class SalesExportFileModel
{
    [Excel("Typ")]
    public string Type { get; init; }

    [Excel("Ferma")]
    public string Farm { get; init; }

    [Excel("Identyfikator (cykl)")]
    public string Cycle { get; init; }

    [Excel("Data sprzedaży", CellFormat = "yyyy-MM-dd")]
    public DateOnly SaleDate { get; init; }

    [Excel("Ubojnia")]
    public string Slaughterhouse { get; init; }

    [Excel("Kurnik")]
    public string Henhouse { get; init; }

    [Excel("Waga ubojni [kg]", CellFormat = "0.00")]
    public decimal Weight { get; init; }

    [Excel("Ilość sztuk ubojnia [szt]")]
    public int Quantity { get; init; }

    [Excel("Konfiskaty [kg]", CellFormat = "0.00")]
    public decimal ConfiscatedWeight { get; init; }

    [Excel("Konfiskaty [szt]")]
    public int ConfiscatedCount { get; init; }

    [Excel("Kurczęta martwe [kg]", CellFormat = "0.00")]
    public decimal DeadWeight { get; init; }

    [Excel("Kurczęta martwe [szt]")]
    public int DeadCount { get; init; }

    [Excel("Waga producenta [kg]", CellFormat = "0.00")]
    public decimal FarmerWeight { get; init; }

    [Excel("Cena bazowa [zł]", CellFormat = "0.00", IsCurrency = true)]
    public decimal BasePrice { get; init; }

    [Excel("Cena z dodatkami [zł]", CellFormat = "0.00", IsCurrency = true)]
    public decimal PriceWithExtras { get; init; }

    [Excel("Komentarz")]
    public string Comment { get; init; }

    [Excel("Inne dodatki", IsList = true)]
    public List<SaleOtherExtrasFileModel> OtherExtras { get; set; }

    [Excel("Data wysyłki do IRZplus", CellFormat = "yyyy-MM-dd HH:mm")]
    public DateTime? DateIrzSentUtc { get; init; }

    [Excel("Numer dokumentu IRZplus")]
    public string DocumentNumber { get; init; }
    
    [Excel("Data utworzenia wpisu", CellFormat = "yyyy-MM-dd HH:mm")]
    public DateTime DateCreated { get; init; }
}

public class SaleOtherExtrasFileModel
{
    [Excel("Nazwa")]
    public string Name { get; init; }

    [Excel("Wartość", CellFormat = "0.00", IsCurrency = true)]
    public decimal Value { get; init; }
}
