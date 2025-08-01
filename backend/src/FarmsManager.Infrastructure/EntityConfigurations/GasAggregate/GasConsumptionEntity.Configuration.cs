using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.GasAggregate;

public class GasConsumptionEntityConfiguration : BaseConfiguration<GasConsumptionEntity>
{
    public override void Configure(EntityTypeBuilder<GasConsumptionEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);

        builder.Property(t => t.Status).HasConversion<string>();

        builder.HasMany(t => t.ConsumptionSources)
            .WithOne(cs => cs.GasConsumption)
            .HasForeignKey(cs => cs.GasConsumptionId);
    }
}