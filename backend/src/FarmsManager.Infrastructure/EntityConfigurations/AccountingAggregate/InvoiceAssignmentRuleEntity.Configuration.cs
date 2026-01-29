using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.AccountingAggregate;

public class InvoiceAssignmentRuleEntityConfiguration : BaseConfiguration<InvoiceAssignmentRuleEntity>
{
    public override void Configure(EntityTypeBuilder<InvoiceAssignmentRuleEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("invoice_assignment_rules");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.Description)
            .IsRequired(false)
            .HasMaxLength(2000);

        builder.Property(r => r.Priority)
            .IsRequired();

        builder.Property(r => r.AssignedUserId)
            .IsRequired();

        builder.Property(r => r.IncludeKeywords)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(r => r.ExcludeKeywords)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(r => r.FarmIds)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relacja z użytkownikiem (pracownikiem)
        builder.HasOne(r => r.AssignedUser)
            .WithMany()
            .HasForeignKey(r => r.AssignedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacja z podmiotem gospodarczym (opcjonalna)
        builder.HasOne(r => r.TaxBusinessEntity)
            .WithMany()
            .HasForeignKey(r => r.TaxBusinessEntityId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(r => r.Priority);
        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => r.AssignedUserId);
        builder.HasIndex(r => new { r.IsActive, r.Priority });
    }
}
