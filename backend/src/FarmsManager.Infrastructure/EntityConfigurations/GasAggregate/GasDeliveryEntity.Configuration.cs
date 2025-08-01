using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.GasAggregate;

public class GasDeliveryEntityConfiguration : BaseConfiguration<GasDeliveryEntity>
{
    public override void Configure(EntityTypeBuilder<GasDeliveryEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.GasContractor).WithMany().HasForeignKey(t => t.GasContractorId);

        builder.Property(t => t.FilePath).IsRequired(false);
    }
}