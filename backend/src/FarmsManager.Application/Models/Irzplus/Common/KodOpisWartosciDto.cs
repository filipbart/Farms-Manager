using System.Text.Json.Serialization;

namespace FarmsManager.Application.Models.Irzplus.Common;

public class KodOpisWartosciDto
{
    /// <summary>
    /// Kod wartości (np. kod gatunku, kod kraju, itp.)
    /// </summary>
    [JsonPropertyName("kod")]
    public string Kod { get; set; }

    /// <summary>
    /// Opis wartości
    /// </summary>
    [JsonPropertyName("opis")]
    public string Opis { get; set; }
}
