using System.Text.Json.Serialization;

namespace FarmsManager.Application.Models.Irzplus.Common;

public class ZlozenieDyspozycjiResponse
{
    /// <summary>
    /// Komunikat zwrotny (np. informacja o powodzeniu)
    /// </summary>
    [JsonPropertyName("komunikat")]
    public string Komunikat { get; set; }

    /// <summary>
    /// Lista błędów walidacyjnych (jeśli wystąpiły)
    /// </summary>
    [JsonPropertyName("bledy")]
    public List<BladWalidacjiDTO> Bledy { get; set; }

    /// <summary>
    /// Numer nadany dokumentowi
    /// </summary>
    [JsonPropertyName("numerDokumentu")]
    public string NumerDokumentu { get; set; }
}
