using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.HatcheryAggregate;

public class HatcheryEntityConfiguration : BaseConfiguration<HatcheryEntity>
{
    public override void Configure(EntityTypeBuilder<HatcheryEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(50);
    }
}