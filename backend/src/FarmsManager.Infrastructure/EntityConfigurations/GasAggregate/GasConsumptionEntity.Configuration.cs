using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.GasAggregate;

public class GasConsumptionEntityConfiguration : BaseConfiguration<GasConsumptionEntity>
{
    public override void Configure(EntityTypeBuilder<GasConsumptionEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);
    }
}