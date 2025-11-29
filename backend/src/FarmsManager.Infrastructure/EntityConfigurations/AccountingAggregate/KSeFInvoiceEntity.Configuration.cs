using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.AccountingAggregate;

public class KSeFInvoiceEntityConfiguration : BaseConfiguration<KSeFInvoiceEntity>
{
    public override void Configure(EntityTypeBuilder<KSeFInvoiceEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.KSeFNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.InvoiceDate)
            .IsRequired();

        builder.Property(t => t.SellerNip)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(t => t.SellerName)
            .IsRequired(false)
            .HasMaxLength(300);

        builder.Property(t => t.BuyerNip)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(t => t.BuyerName)
            .IsRequired(false)
            .HasMaxLength(300);

        builder.Property(t => t.InvoiceType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.RelatedInvoiceNumber)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(t => t.RelatedInvoiceId)
            .IsRequired(false);

        builder.Property(t => t.Comment)
            .IsRequired(false)
            .HasMaxLength(2000);


        builder.Property(t => t.InvoiceXml)
            .IsRequired()
            .HasColumnType("text");

        builder.HasOne(t => t.AssignedUser)
            .WithMany()
            .HasForeignKey(t => t.AssignedUserId);

        // Indexes
        builder.HasIndex(t => t.KSeFNumber).IsUnique();
        builder.HasIndex(t => t.InvoiceNumber);
        builder.HasIndex(t => t.InvoiceDate);
    }
}