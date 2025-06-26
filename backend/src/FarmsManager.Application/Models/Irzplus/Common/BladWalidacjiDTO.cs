using System.Text.Json.Serialization;

namespace FarmsManager.Application.Models.Irzplus.Common;

public class BladWalidacjiDTO
{
    /// <summary>
    /// Kod błędu
    /// </summary>
    [JsonPropertyName("kodBledu")]
    public string KodBledu { get; set; }

    /// <summary>
    /// Komunikat błędu
    /// </summary>
    [JsonPropertyName("komunikat")]
    public string Komunikat { get; set; }
}
