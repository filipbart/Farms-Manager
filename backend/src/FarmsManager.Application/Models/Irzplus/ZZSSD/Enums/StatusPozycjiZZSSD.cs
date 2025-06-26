using System.Text.Json.Serialization;

namespace FarmsManager.Application.Models.Irzplus.ZZSSD.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusPozycjiZZSSD
{
    [JsonPropertyName("ZATWIERDZONA")]
    Zatwierdzona,

    [JsonPropertyName("DO_ZATWIERDZENIA")]
    DoZatwierdzenia,

    [JsonPropertyName("POMINIETA")]
    Pominieta
}
