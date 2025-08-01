using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.EmployeeAggregate;

public class EmployeeEntityConfiguration : BaseConfiguration<EmployeeEntity>
{
    public override void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Farm)
            .WithMany(e => e.Employees)
            .HasForeignKey(e => e.FarmId);

        builder.HasMany(e => e.Files)
            .WithOne(f => f.Employee)
            .HasForeignKey(f => f.EmployeeId);

        builder.HasMany(e => e.Reminders)
            .WithOne(r => r.Employee)
            .HasForeignKey(r => r.EmployeeId);
    }
}