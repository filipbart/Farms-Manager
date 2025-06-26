using System.Text.Json;
using System.Text.Json.Serialization;

namespace FarmsManager.Shared.Extensions;

public static class JsonExtensions
{
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = false,
        MaxDepth = 64,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters = { new JsonStringEnumConverter(), new TimeOnlyConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals
    };


    public static readonly JsonSerializerOptions DefaultOptionsIndented = new(DefaultOptions)
    {
        WriteIndented = true
    };

    public static readonly JsonSerializerOptions DefaultOptionsWithoutCamelCase = new(DefaultOptions)
    {
        PropertyNamingPolicy = null
    };

    public static readonly JsonSerializerOptions DefaultOptionsWithNulls = new(DefaultOptions)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    public static T ParseJsonString<T>(this string text, JsonSerializerOptions options = null) =>
        JsonSerializer.Deserialize<T>(text, options ?? DefaultOptions);

    public static string ToJsonString(this object obj, bool withCamelCase = true)
    {
        return JsonSerializer.Serialize(obj, withCamelCase ? DefaultOptions : DefaultOptionsWithoutCamelCase);
    }
    
    public static string ToJsonStringWithNulls(this object obj)
    {
        return JsonSerializer.Serialize(obj, DefaultOptionsWithNulls);
    }
}

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    private readonly string _serializationFormat;

    public TimeOnlyConverter() : this(null)
    {
    }

    public TimeOnlyConverter(string serializationFormat)
    {
        _serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
    }

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return TimeOnly.Parse(value!);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString(_serializationFormat));
}