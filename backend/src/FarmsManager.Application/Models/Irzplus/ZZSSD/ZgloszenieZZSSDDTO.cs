using FarmsManager.Application.Models.Irzplus.Common;

namespace FarmsManager.Application.Models.Irzplus.ZZSSD;

using System.Text.Json.Serialization;

public class ZgloszenieZZSSDDTO
{
    /// <summary>
    /// Lista pozycji zgłoszenia
    /// </summary>
    [JsonPropertyName("pozycje")]
    public List<PozycjaZZSSDDTO> Pozycje { get; set; }

    /// <summary>
    /// Gatunek drobiu
    /// </summary>
    [JsonPropertyName("gatunek")]
    public KodOpisWartosciDto Gatunek { get; set; }

    /// <summary>
    /// Do działalności
    /// </summary>
    [JsonPropertyName("doDzialalnosci")]
    public string DoDzialalnosci { get; set; }

    /// <summary>
    /// Typ zdarzenia
    /// </summary>
    [JsonPropertyName("typZdarzenia")]
    public KodOpisWartosciDto TypZdarzenia { get; set; }

    /// <summary>
    /// Data zdarzenia
    /// </summary>
    [JsonPropertyName("dataZdarzenia")]
    public DateOnly DataZdarzenia { get; set; }

    /// <summary>
    /// Liczba drobiu (przybyło)
    /// </summary>
    [JsonPropertyName("liczbaDrobiuPrzybylo")]
    public int? LiczbaDrobiuPrzybylo { get; set; }

    /// <summary>
    /// Liczba jaj wylęgowych (przybyło)
    /// </summary>
    [JsonPropertyName("liczbaJajWylegowychPrzybylo")]
    public int? LiczbaJajWylegowychPrzybylo { get; set; }

    /// <summary>
    /// Kod kraju
    /// </summary>
    [JsonPropertyName("kodKraju")]
    public KodOpisWartosciDto KodKraju { get; set; }
}