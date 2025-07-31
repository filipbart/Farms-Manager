using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.GasAggregate;

public class GasContractorEntityConfiguration : BaseConfiguration<GasContractorEntity>
{
    public override void Configure(EntityTypeBuilder<GasContractorEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);
    }
}