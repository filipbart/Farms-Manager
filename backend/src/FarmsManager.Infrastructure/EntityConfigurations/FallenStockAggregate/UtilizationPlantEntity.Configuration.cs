using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FallenStockAggregate;

public class UtilizationPlantEntityConfiguration : BaseConfiguration<UtilizationPlantEntity>
{
    public override void Configure(EntityTypeBuilder<UtilizationPlantEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(50);
    }
}