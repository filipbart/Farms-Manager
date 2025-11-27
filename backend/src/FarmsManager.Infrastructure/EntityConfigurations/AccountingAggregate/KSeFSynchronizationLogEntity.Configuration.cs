using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.AccountingAggregate;

public class KSeFSynchronizationLogEntityConfiguration : BaseConfiguration<KSeFSynchronizationLogEntity>
{
    public override void Configure(EntityTypeBuilder<KSeFSynchronizationLogEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.StartedAt)
            .IsRequired();

        builder.Property(t => t.CompletedAt)
            .IsRequired(false);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.InvoicesDownloaded)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.InvoicesSaved)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.ErrorsCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.ErrorMessage)
            .IsRequired(false)
            .HasMaxLength(2000);

        builder.Property(t => t.IsManual)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.DurationSeconds)
            .IsRequired(false);

        // Index dla lepszej wydajności przy pobieraniu ostatnich synchronizacji
        builder.HasIndex(t => t.StartedAt)
            .IsDescending();

        // Index dla statusu
        builder.HasIndex(t => t.Status);
        
        // Relacje do audit fields (Creator, Modifier, Deleter)
        builder.HasOne(t => t.Creator)
            .WithMany()
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(t => t.Modifier)
            .WithMany()
            .HasForeignKey(t => t.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(t => t.Deleter)
            .WithMany()
            .HasForeignKey(t => t.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
