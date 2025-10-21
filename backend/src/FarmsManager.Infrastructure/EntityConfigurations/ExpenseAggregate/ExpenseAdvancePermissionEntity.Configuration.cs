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

        builder.Property(t => t.EmployeeId)
            .IsRequired();

        builder.Property(t => t.PermissionType)
            .IsRequired();

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Employee)
            .WithMany()
            .HasForeignKey(t => t.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.UserId, t.EmployeeId, t.PermissionType })
            .IsUnique()
            .HasDatabaseName("IX_ExpenseAdvancePermissions_UserEmployeeType").HasFilter("date_deleted_utc IS NULL");
    }
}