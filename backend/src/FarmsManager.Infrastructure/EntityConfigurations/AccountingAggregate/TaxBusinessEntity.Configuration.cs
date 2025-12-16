using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.AccountingAggregate;

public class TaxBusinessEntityConfiguration : BaseConfiguration<TaxBusinessEntity>
{
    public override void Configure(EntityTypeBuilder<TaxBusinessEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("tax_business_entity");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Nip)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(t => t.BusinessType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.Description)
            .IsRequired(false)
            .HasMaxLength(2000);

        // Relacja z fakturami KSeF
        builder.HasMany(t => t.Invoices)
            .WithOne(i => i.TaxBusinessEntity)
            .HasForeignKey(i => i.TaxBusinessEntityId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacja z fermami
        builder.HasMany(t => t.Farms)
            .WithOne(f => f.TaxBusinessEntity)
            .HasForeignKey(f => f.TaxBusinessEntityId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(t => t.Nip);
        builder.HasIndex(t => new { t.Nip, t.Name });
    }
}
