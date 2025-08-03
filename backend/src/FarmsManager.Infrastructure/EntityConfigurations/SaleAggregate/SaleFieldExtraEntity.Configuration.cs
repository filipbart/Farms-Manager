using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.SaleAggregate;

public class SaleFieldExtraEntityConfiguration : BaseConfiguration<SaleFieldExtraEntity>
{
    public override void Configure(EntityTypeBuilder<SaleFieldExtraEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(50);
    }
}