using FarmsManager.Application.Models.Irzplus.Common;
using FarmsManager.Application.Models.Irzplus.ZZSSD.Enums;

namespace FarmsManager.Application.Models.Irzplus.ZZSSD;

using System.Text.Json.Serialization;

public class PozycjaZZSSDDTO
{
    /// <summary>
    /// Numer porządkowy (lp)
    /// </summary>
    [JsonPropertyName("lp")]
    public long Lp { get; set; }

    /// <summary>
    /// Status pozycji (ZATWIERDZONA, DO_ZATWIERDZENIA, POMINIETA)
    /// </summary>
    [JsonPropertyName("statusPozycji")]
    public string StatusPozycji { get; set; }

    /// <summary>
    /// Numer identyfikacyjny partii drobiu
    /// </summary>
    [JsonPropertyName("numerIdenPartiiDrobiu")]
    public string NumerIdenPartiiDrobiu { get; set; }

    /// <summary>
    /// Liczba drobiu (ubyło)
    /// </summary>
    [JsonPropertyName("liczbaDrobiuUbylo")]
    public int? LiczbaDrobiuUbylo { get; set; }

    /// <summary>
    /// Liczba jaj wylęgowych (ubyło)
    /// </summary>
    [JsonPropertyName("liczbaJajWylegowychUbylo")]
    public int? LiczbaJajWylegowychUbylo { get; set; }

    /// <summary>
    /// Kategoria jaj wylęgowych
    /// </summary>
    [JsonPropertyName("kategoriaJajWylegowych")]
    public KodOpisWartosciDto KategoriaJajWylegowych { get; set; }

    /// <summary>
    /// Numer identyfikacyjny partii drobiu spoza kraju
    /// </summary>
    [JsonPropertyName("numerIdenPartiiDrobiuSpozaKraju")]
    public string NumerIdenPartiiDrobiuSpozaKraju { get; set; }

    /// <summary>
    /// Wnioskodawca przewoźnika
    /// </summary>
    [JsonPropertyName("wniPrzewoznika")]
    public string WniPrzewoznika { get; set; }

    /// <summary>
    /// Rodzaj środka transportu
    /// </summary>
    [JsonPropertyName("rodzajSrodkaTransportu")]
    public KodOpisWartosciDto RodzajSrodkaTransportu { get; set; }

    /// <summary>
    /// Numer rejestracyjny środka transportu
    /// </summary>
    [JsonPropertyName("nrRejestracyjnySrodkaTransportu")]
    public string NrRejestracyjnySrodkaTransportu { get; set; }

    /// <summary>
    /// Transport własny
    /// </summary>
    [JsonPropertyName("transportWlasny")]
    public bool TransportWlasny { get; set; }

    /// <summary>
    /// Budynek
    /// </summary>
    [JsonPropertyName("budynek")]
    public KodOpisWartosciDto Budynek { get; set; }

    /// <summary>
    /// Oznaczenie wsadu
    /// </summary>
    [JsonPropertyName("oznaczenieWsadu")]
    public string OznaczenieWsadu { get; set; }

    /// <summary>
    /// Masa ciała drobiu lub jaj wylęgowych
    /// </summary>
    [JsonPropertyName("masaCialaDrobiuJajWylegowych")]
    public double? MasaCialaDrobiuJajWylegowych { get; set; }

    /// <summary>
    /// Z działalności
    /// </summary>
    [JsonPropertyName("zDzialalnosci")]
    public string ZDzialalnosci { get; set; }
}
