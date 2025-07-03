using System.Text.Json;
using System.Text.Json.Serialization;
using FarmsManager.Infrastructure.Helpers;
using FarmsManager.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations;

public class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : class
{
    protected JsonSerializerOptions JsonOptions { get; } = new()
    {
        WriteIndented = false,
        MaxDepth = 64,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters = { new JsonStringEnumConverter(), new TimeOnlyConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString |
                         JsonNumberHandling.AllowNamedFloatingPointLiterals
    };

    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        var tableName = NamingHelper.PascalCaseToSnakeCase(typeof(T).Name.Replace("Entity", ""));
        builder.ToTable(tableName);
    }
}