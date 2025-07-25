using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FeedAggregate;

public class FeedInvoiceEntityConfiguration : BaseConfiguration<FeedInvoiceEntity>
{
    public override void Configure(EntityTypeBuilder<FeedInvoiceEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Comment).IsRequired(false);

        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Henhouse).WithMany().HasForeignKey(t => t.HenhouseId);
        builder.HasOne(t => t.InvoiceCorrection).WithMany().HasForeignKey(t => t.InvoiceCorrectionId);
        builder.HasOne(t => t.Payment).WithMany().HasForeignKey(t => t.PaymentId);
    }
}