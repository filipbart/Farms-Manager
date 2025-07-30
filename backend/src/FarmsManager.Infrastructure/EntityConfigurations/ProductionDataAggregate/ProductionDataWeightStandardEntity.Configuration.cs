using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ProductionDataAggregate;

public class ProductionDataWeightStandardEntityConfiguration : BaseConfiguration<ProductionDataWeightStandardEntity>
{
    public override void Configure(EntityTypeBuilder<ProductionDataWeightStandardEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Day).IsRequired();
        builder.Property(t => t.Weight).IsRequired();
    }
}