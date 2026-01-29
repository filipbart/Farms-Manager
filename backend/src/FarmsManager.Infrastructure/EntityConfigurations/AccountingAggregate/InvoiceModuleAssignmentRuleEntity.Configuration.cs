using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.AccountingAggregate;

public class InvoiceModuleAssignmentRuleEntityConfiguration : BaseConfiguration<InvoiceModuleAssignmentRuleEntity>
{
    public override void Configure(EntityTypeBuilder<InvoiceModuleAssignmentRuleEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.Description)
            .IsRequired(false)
            .HasMaxLength(2000);

        builder.Property(r => r.Priority)
            .IsRequired();

        builder.Property(r => r.TargetModule)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.IncludeKeywords)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(r => r.ExcludeKeywords)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(r => r.InvoiceDirection)
            .IsRequired(false)
            .HasConversion<string>();

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relacja z podmiotem gospodarczym (opcjonalna)
        builder.HasOne(r => r.TaxBusinessEntity)
            .WithMany()
            .HasForeignKey(r => r.TaxBusinessEntityId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacja z fermą (opcjonalna)
        builder.HasOne(r => r.Farm)
            .WithMany()
            .HasForeignKey(r => r.FarmId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(r => r.Priority);
        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => r.TargetModule);
        builder.HasIndex(r => new { r.IsActive, r.Priority });
    }
}
