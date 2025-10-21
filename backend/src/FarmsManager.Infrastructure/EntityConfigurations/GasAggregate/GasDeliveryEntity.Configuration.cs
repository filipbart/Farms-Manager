using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using Microsoft.EntityFrameworkCore;
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
        builder.HasOne(t => t.Creator).WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Modifier).WithMany().HasForeignKey(t => t.ModifiedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Deleter).WithMany().HasForeignKey(t => t.DeletedBy).OnDelete(DeleteBehavior.Restrict);
        builder.Property(t => t.FilePath).IsRequired(false);
    }
}