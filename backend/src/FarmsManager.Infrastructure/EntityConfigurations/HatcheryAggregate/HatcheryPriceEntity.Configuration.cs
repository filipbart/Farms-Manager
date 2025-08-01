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

        builder.HasIndex(t => new { t.HatcheryId, t.Date }).IsUnique().HasFilter("date_deleted_utc IS NULL");

        builder.HasOne(t => t.Hatchery).WithMany().HasForeignKey(t => t.HatcheryId);
    }
}