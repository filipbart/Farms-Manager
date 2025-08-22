using System.Text.Json.Serialization;

namespace FarmsManager.Application.Models.Irzplus.Common;

/// <summary>
/// Główna klasa odpowiedzi z API, zawierająca komunikat i listę danych o drobiu.
/// </summary>
public class PobieranieZwierzatApiResponse
{
    [JsonPropertyName("komunikat")] public string Komunikat { get; set; }

    [JsonPropertyName("listaDrob")] public List<ListaDrobItem> ListaDrob { get; set; }
}

/// <summary>
/// Reprezentuje pojedynczy wpis na liście danych o drobiu.
/// </summary>
public class ListaDrobItem
{
    [JsonPropertyName("lp")] public int? Lp { get; set; }

    [JsonPropertyName("numerDzialalnosci")]
    public string NumerDzialalnosci { get; set; }

    [JsonPropertyName("numerPartiiDrobiu")]
    public string NumerPartiiDrobiu { get; set; }

    [JsonPropertyName("gatunek")] public Gatunek Gatunek { get; set; }

    [JsonPropertyName("liczbaDrobiu")] public int? LiczbaDrobiu { get; set; }

    [JsonPropertyName("ogolnaLiczbaDrobiu")]
    public int? OgolnaLiczbaDrobiu { get; set; }

    [JsonPropertyName("liczbaPrzybycDrobiu")]
    public int? LiczbaPrzybycDrobiu { get; set; }

    [JsonPropertyName("liczbaWybycDrobiu")]
    public int? LiczbaWybycDrobiu { get; set; }

    [JsonPropertyName("liczbaNieprawidlowosciDrobiu")]
    public int? LiczbaNieprawidlowosciDrobiu { get; set; }

    [JsonPropertyName("liczbaJajWylegowych")]
    public int? LiczbaJajWylegowych { get; set; }

    [JsonPropertyName("ogolnaLiczbaJajWylegowych")]
    public int? OgolnaLiczbaJajWylegowych { get; set; }

    [JsonPropertyName("liczbaPrzybycJajWylegowych")]
    public int? LiczbaPrzybycJajWylegowych { get; set; }

    [JsonPropertyName("liczbaWybycJajWylegowych")]
    public int? LiczbaWybycJajWylegowych { get; set; }

    [JsonPropertyName("liczbaNieprawidlowosciJajWylegowych")]
    public int? LiczbaNieprawidlowosciJajWylegowych { get; set; }
}

/// <summary>
/// Reprezentuje zagnieżdżony obiekt z informacjami o gatunku.
/// </summary>
public class Gatunek
{
    [JsonPropertyName("kod")] public string Kod { get; set; }

    [JsonPropertyName("opis")] public string Opis { get; set; }
}