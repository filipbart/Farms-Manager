using FarmsManager.Domain.Aggregates.SeedWork.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.SeedWork;

public class ColumnViewEntityConfiguration : BaseConfiguration<ColumnViewEntity>
{
    public override void Configure(EntityTypeBuilder<ColumnViewEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(40);
        builder.Property(t => t.State).IsRequired(false).HasColumnType("jsonb");
    }
}