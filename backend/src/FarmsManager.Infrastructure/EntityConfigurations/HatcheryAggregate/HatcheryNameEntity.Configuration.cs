using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.HatcheryAggregate;

public class HatcheryNameEntityConfiguration : BaseConfiguration<HatcheryNameEntity>
{
    public override void Configure(EntityTypeBuilder<HatcheryNameEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(50);
    }
}