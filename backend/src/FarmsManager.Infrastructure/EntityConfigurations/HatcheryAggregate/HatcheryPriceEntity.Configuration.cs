using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.HatcheryAggregate;

public class HatcheryPriceEntityConfiguration : BaseConfiguration<HatcheryPriceEntity>
{
    public override void Configure(EntityTypeBuilder<HatcheryPriceEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => new { t.HatcheryName, t.Date }).IsUnique().HasFilter("date_deleted_utc IS NULL");
        builder.HasOne(t => t.Creator).WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Modifier).WithMany().HasForeignKey(t => t.ModifiedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Deleter).WithMany().HasForeignKey(t => t.DeletedBy).OnDelete(DeleteBehavior.Restrict);
    }
}