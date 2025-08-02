using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.EmployeeAggregate;

public class EmployeeReminderEntityConfiguration : BaseConfiguration<EmployeeReminderEntity>
{
    public override void Configure(EntityTypeBuilder<EmployeeReminderEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Title).IsRequired().HasMaxLength(255);
        builder.Property(f => f.DueDate).IsRequired();
    }
}