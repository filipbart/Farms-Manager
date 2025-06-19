namespace FarmsManager.Shared.Extensions;

public static class StringExtensions
{
    public static bool IsNotEmpty(this string value) => !string.IsNullOrWhiteSpace(value);
    public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);
}