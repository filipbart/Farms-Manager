using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ProductionDataAggregate;

public class ProductionDataRemainingFeedEntityConfiguration : BaseConfiguration<ProductionDataRemainingFeedEntity>
{
    public override void Configure(EntityTypeBuilder<ProductionDataRemainingFeedEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Henhouse).WithMany().HasForeignKey(t => t.HenhouseId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
    }
}