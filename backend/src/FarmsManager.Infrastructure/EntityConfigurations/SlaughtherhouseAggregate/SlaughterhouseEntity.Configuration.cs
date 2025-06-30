using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.SlaughtherhouseAggregate;

public class SlaughterhouseEntityConfiguration : BaseConfiguration<SlaughterhouseEntity>
{
    public override void Configure(EntityTypeBuilder<SlaughterhouseEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(50);
    }
}