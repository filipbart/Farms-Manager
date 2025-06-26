using System.ComponentModel;
using System.Runtime.Serialization;

namespace FarmsManager.Application.Models.Irzplus.ZZSSD.Enums;

public enum KategoriaJajWylegowych
{
    [EnumMember(Value = "KJIN")] [Description("Inny")]
    Inny,

    [EnumMember(Value = "KJMIE")] [Description("Mięsna")]
    Miesna,

    [EnumMember(Value = "KJNIE")] [Description("Nieśna")]
    Niesna
}