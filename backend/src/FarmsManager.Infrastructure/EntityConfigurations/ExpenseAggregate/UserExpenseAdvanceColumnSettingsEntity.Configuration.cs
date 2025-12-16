using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ExpenseAggregate;

public class UserExpenseAdvanceColumnSettingsEntityConfiguration : BaseConfiguration<UserExpenseAdvanceColumnSettingsEntity>
{
    public override void Configure(EntityTypeBuilder<UserExpenseAdvanceColumnSettingsEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.VisibleColumns)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.UserId)
            .IsUnique()
            .HasDatabaseName("IX_UserExpenseAdvanceColumnSettings_UserId")
            .HasFilter("date_deleted_utc IS NULL");
    }
}
