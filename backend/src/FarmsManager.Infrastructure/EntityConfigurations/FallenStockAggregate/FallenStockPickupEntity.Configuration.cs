using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FallenStockAggregate;

public class FallenStockPickupEntityConfiguration : BaseConfiguration<FallenStockPickupEntity>
{
    public override void Configure(EntityTypeBuilder<FallenStockPickupEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => new { t.CycleId, t.FarmId, t.Date }).IsUnique()
            .HasFilter("date_deleted_utc IS NULL");
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.Creator).WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Modifier).WithMany().HasForeignKey(t => t.ModifiedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Deleter).WithMany().HasForeignKey(t => t.DeletedBy).OnDelete(DeleteBehavior.Restrict);
    }
}