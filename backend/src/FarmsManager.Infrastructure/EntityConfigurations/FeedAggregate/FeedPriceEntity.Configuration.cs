using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FeedAggregate;

public class FeedPriceEntityConfiguration : BaseConfiguration<FeedPriceEntity>
{
    public override void Configure(EntityTypeBuilder<FeedPriceEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(50);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
    }
}