using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FarmAggregate;

public class HenhouseEntityConfiguration : BaseConfiguration<HenhouseEntity>
{
    public override void Configure(EntityTypeBuilder<HenhouseEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(50);
    }
}