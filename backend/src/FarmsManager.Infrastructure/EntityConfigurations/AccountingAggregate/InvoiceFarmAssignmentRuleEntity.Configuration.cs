using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.AccountingAggregate;

public class InvoiceFarmAssignmentRuleEntityConfiguration : IEntityTypeConfiguration<InvoiceFarmAssignmentRuleEntity>
{
    public void Configure(EntityTypeBuilder<InvoiceFarmAssignmentRuleEntity> builder)
    {
        builder.ToTable("InvoiceFarmAssignmentRules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.TargetFarmId)
            .IsRequired();

        builder.Property(x => x.IncludeKeywords)
            .HasColumnType("jsonb");

        builder.Property(x => x.ExcludeKeywords)
            .HasColumnType("jsonb");

        builder.Property(x => x.InvoiceDirection)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.TargetFarm)
            .WithMany()
            .HasForeignKey(x => x.TargetFarmId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TaxBusinessEntity)
            .WithMany()
            .HasForeignKey(x => x.TaxBusinessEntityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.Priority);
        builder.HasIndex(x => x.IsActive);
    }
}
