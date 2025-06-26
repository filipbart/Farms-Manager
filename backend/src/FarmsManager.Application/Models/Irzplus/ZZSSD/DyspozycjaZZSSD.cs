using System.Text.Json.Serialization;

namespace FarmsManager.Application.Models.Irzplus.ZZSSD;

public class DyspozycjaZZSSD
{
    /// <summary>
    /// Komórka organizacyjna ARiMR, do której składana jest dyspozycja
    /// </summary>
    [JsonPropertyName("komorkaOrganizacyjna")]
    public string KomorkaOrganizacyjna { get; set; }

    /// <summary>
    /// Numer producenta, który składa dyspozycję
    /// </summary>
    [JsonPropertyName("numerProducenta")]
    public string NumerProducenta { get; set; }

    /// <summary>
    /// Zgłoszenie (nested obiekt)
    /// </summary>
    [JsonPropertyName("zgloszenie")]
    public ZgloszenieZZSSDDTO Zgloszenie { get; set; }
}