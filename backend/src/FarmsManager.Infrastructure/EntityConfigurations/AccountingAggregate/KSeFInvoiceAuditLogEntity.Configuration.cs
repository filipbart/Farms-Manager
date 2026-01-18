using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.AccountingAggregate;

public class KSeFInvoiceAuditLogEntityConfiguration : BaseConfiguration<KSeFInvoiceAuditLogEntity>
{
    public override void Configure(EntityTypeBuilder<KSeFInvoiceAuditLogEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("ksef_invoice_audit_log");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.InvoiceId)
            .IsRequired();

        builder.Property(t => t.Action)
            .IsRequired();

        builder.Property(t => t.PreviousStatus)
            .IsRequired(false);

        builder.Property(t => t.NewStatus)
            .IsRequired(false);

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.UserName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Comment)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.HasOne(t => t.Invoice)
            .WithMany(i => i.AuditLogs)
            .HasForeignKey(t => t.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(t => t.InvoiceId);
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.DateCreatedUtc);
        builder.HasIndex(t => t.Action);
    }
}
