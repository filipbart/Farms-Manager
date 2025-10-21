using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FeedAggregate;

public class FeedPaymentEntityConfiguration : BaseConfiguration<FeedPaymentEntity>
{
    public override void Configure(EntityTypeBuilder<FeedPaymentEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Status).IsRequired().HasDefaultValue(FeedPaymentStatus.Unrealized);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Creator).WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Modifier).WithMany().HasForeignKey(t => t.ModifiedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Deleter).WithMany().HasForeignKey(t => t.DeletedBy).OnDelete(DeleteBehavior.Restrict);
    }
}