using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.AccountingAggregate;

public class KSeFInvoiceRelationEntityConfiguration : BaseConfiguration<KSeFInvoiceRelationEntity>
{
    public override void Configure(EntityTypeBuilder<KSeFInvoiceRelationEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("ksef_invoice_relation");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.SourceInvoiceId)
            .IsRequired();

        builder.Property(t => t.TargetInvoiceId)
            .IsRequired();

        builder.Property(t => t.RelationType)
            .IsRequired();

        builder.HasOne(t => t.SourceInvoice)
            .WithMany(i => i.SourceRelations)
            .HasForeignKey(t => t.SourceInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.TargetInvoice)
            .WithMany(i => i.TargetRelations)
            .HasForeignKey(t => t.TargetInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.SourceInvoiceId);
        builder.HasIndex(t => t.TargetInvoiceId);
        builder.HasIndex(t => new { t.SourceInvoiceId, t.TargetInvoiceId }).IsUnique();
    }
}
