using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.GasAggregate;

public class GasConsumptionSourceEntityConfiguration : BaseConfiguration<GasConsumptionSourceEntity>
{
    public override void Configure(EntityTypeBuilder<GasConsumptionSourceEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);


        builder.HasOne(t => t.GasConsumption)
            .WithMany(c => c.ConsumptionSources)
            .HasForeignKey(t => t.GasConsumptionId);


        builder.HasOne(t => t.GasDelivery)
            .WithMany()
            .HasForeignKey(t => t.GasDeliveryId);
    }
}