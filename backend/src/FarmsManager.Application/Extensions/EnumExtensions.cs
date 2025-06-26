using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using FarmsManager.Application.Models.Irzplus.Common;

namespace FarmsManager.Application.Extensions;

public static class EnumExtensions
{
    public static string GetEnumMemberValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? value.ToString();
    }

    public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
    {
        var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
    }

    public static KodOpisWartosciDto ToKodOpisDto<TEnum>(this TEnum value) where TEnum : Enum
    {
        return new KodOpisWartosciDto
        {
            Kod = value.GetEnumMemberValue(),
            Opis = value.GetDescription()
        };
    }
}