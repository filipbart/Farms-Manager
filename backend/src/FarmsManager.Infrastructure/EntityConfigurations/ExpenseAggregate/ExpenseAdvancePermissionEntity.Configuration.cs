using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ExpenseAggregate;

public class ExpenseAdvancePermissionEntityConfiguration : BaseConfiguration<ExpenseAdvancePermissionEntity>
{
    public override void Configure(EntityTypeBuilder<ExpenseAdvancePermissionEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.ExpenseAdvanceRegistryId)
            .IsRequired();

        builder.Property(t => t.PermissionType)
            .IsRequired();

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.ExpenseAdvanceRegistry)
            .WithMany(r => r.Permissions)
            .HasForeignKey(t => t.ExpenseAdvanceRegistryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.UserId, t.ExpenseAdvanceRegistryId, t.PermissionType })
            .IsUnique()
            .HasDatabaseName("IX_ExpenseAdvancePermissions_UserRegistryType");

        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("IX_ExpenseAdvancePermissions_UserId");

        builder.HasIndex(t => t.ExpenseAdvanceRegistryId)
            .HasDatabaseName("IX_ExpenseAdvancePermissions_RegistryId");
    }
}
