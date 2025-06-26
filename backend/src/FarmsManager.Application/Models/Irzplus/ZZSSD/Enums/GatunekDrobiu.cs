using System.ComponentModel;
using System.Runtime.Serialization;

namespace FarmsManager.Application.Models.Irzplus.ZZSSD.Enums;

public enum GatunekDrobiu
{
    [EnumMember(Value = "GESZ")]
    [Description("gęsi")]
    Gesi,

    [EnumMember(Value = "GESG")]
    [Description("gęsi garbonose")]
    GesiGarbonose,

    [EnumMember(Value = "INDY")]
    [Description("indyki")]
    Indyki,

    [EnumMember(Value = "KACZ")]
    [Description("kaczki")]
    Kaczki,

    [EnumMember(Value = "KACP")]
    [Description("kaczki piżmowe")]
    KaczkiPizmowe,

    [EnumMember(Value = "KURY")]
    [Description("kury")]
    Kury,

    [EnumMember(Value = "PERL")]
    [Description("perliczki")]
    Perliczki,

    [EnumMember(Value = "PRZE")]
    [Description("przepiórki japońskie")]
    PrzepiorkiJaponskie,

    [EnumMember(Value = "STRU")]
    [Description("strusie")]
    Strusie
}