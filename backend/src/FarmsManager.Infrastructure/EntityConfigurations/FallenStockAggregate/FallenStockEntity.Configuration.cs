using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FallenStockAggregate;

public class FallenStockEntityConfiguration : BaseConfiguration<FallenStockEntity>
{
    public override void Configure(EntityTypeBuilder<FallenStockEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => new { t.CycleId, t.FarmId, t.HenhouseId, t.UtilizationPlantId }).IsUnique()
            .HasFilter("date_deleted_utc IS NULL");

        builder.HasOne(t => t.Henhouse).WithMany().HasForeignKey(t => t.HenhouseId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.UtilizationPlant).WithMany().HasForeignKey(t => t.UtilizationPlantId);
    }
}