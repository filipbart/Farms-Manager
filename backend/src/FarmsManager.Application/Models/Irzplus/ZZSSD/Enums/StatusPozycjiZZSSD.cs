using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FarmsManager.Application.Models.Irzplus.ZZSSD.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusPozycjiZZSSD
{
    [EnumMember(Value = "ZATWIERDZONA")] 
    Zatwierdzona,

    [EnumMember(Value = "DO_ZATWIERDZENIA")]
    DoZatwierdzenia,

    [EnumMember(Value = "POMINIETA")] 
    Pominieta
}