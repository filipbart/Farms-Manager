using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;
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
        
        builder.HasOne<UserEntity>(t => t.Creator).WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UserEntity>(t => t.Modifier).WithMany().HasForeignKey(t => t.ModifiedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UserEntity>(t => t.Deleter).WithMany().HasForeignKey(t => t.DeletedBy).OnDelete(DeleteBehavior.Restrict);
    }
}