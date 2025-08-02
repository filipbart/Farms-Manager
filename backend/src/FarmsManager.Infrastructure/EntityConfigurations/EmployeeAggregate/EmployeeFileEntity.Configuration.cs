using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.EmployeeAggregate;

public class EmployeeFileEntityConfiguration : BaseConfiguration<EmployeeFileEntity>
{
    public override void Configure(EntityTypeBuilder<EmployeeFileEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName).IsRequired().HasMaxLength(255);
        builder.Property(f => f.FilePath).IsRequired();
        builder.Property(f => f.ContentType).IsRequired().HasMaxLength(100);
    }
}