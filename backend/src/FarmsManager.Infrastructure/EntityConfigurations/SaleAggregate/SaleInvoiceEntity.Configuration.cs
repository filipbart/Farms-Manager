using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.SaleAggregate;

public class SaleInvoiceEntityConfiguration : BaseConfiguration<SaleInvoiceEntity>
{
    public override void Configure(EntityTypeBuilder<SaleInvoiceEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.Slaughterhouse).WithMany().HasForeignKey(t => t.SlaughterhouseId);
    }
}