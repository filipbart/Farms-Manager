using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.AccountingAggregate;

public class KSeFInvoiceAttachmentEntityConfiguration : BaseConfiguration<KSeFInvoiceAttachmentEntity>
{
    public override void Configure(EntityTypeBuilder<KSeFInvoiceAttachmentEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("ksef_invoice_attachment");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.InvoiceId)
            .IsRequired();

        builder.Property(t => t.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(t => t.FileSize)
            .IsRequired();

        builder.Property(t => t.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.UploadedBy)
            .IsRequired();

        builder.HasOne(t => t.Invoice)
            .WithMany(i => i.Attachments)
            .HasForeignKey(t => t.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Uploader)
            .WithMany()
            .HasForeignKey(t => t.UploadedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(t => t.InvoiceId);
        builder.HasIndex(t => t.UploadedBy);
        builder.HasIndex(t => t.DateCreatedUtc);
    }
}
