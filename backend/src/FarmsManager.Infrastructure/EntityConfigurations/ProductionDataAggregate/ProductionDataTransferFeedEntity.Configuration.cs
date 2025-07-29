using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ProductionDataAggregate;

public class ProductionDataTransferFeedEntityConfiguration : BaseConfiguration<ProductionDataTransferFeedEntity>
{
    public override void Configure(EntityTypeBuilder<ProductionDataTransferFeedEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.FromHenhouse).WithMany().HasForeignKey(t => t.FromHenhouseId);
        builder.HasOne(t => t.FromCycle).WithMany().HasForeignKey(t => t.FromCycleId);
        builder.HasOne(t => t.FromFarm).WithMany().HasForeignKey(t => t.FromFarmId);
        builder.HasOne(t => t.ToHenhouse).WithMany().HasForeignKey(t => t.ToHenhouseId);
        builder.HasOne(t => t.ToCycle).WithMany().HasForeignKey(t => t.ToCycleId);
        builder.HasOne(t => t.ToFarm).WithMany().HasForeignKey(t => t.ToFarmId);
    }
}