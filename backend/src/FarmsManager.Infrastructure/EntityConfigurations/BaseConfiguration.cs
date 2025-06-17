using FarmsManager.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations;

public class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : class
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        var tableName = NamingHelper.PascalCaseToSnakeCase(typeof(T).Name.Replace("Entity", ""));
        builder.ToTable(tableName);
    }
}