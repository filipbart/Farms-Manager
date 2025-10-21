using FarmsManager.Domain.Aggregates.EmployeeAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.EmployeeAggregate;

public class EmployeePayslipEntityConfiguration : BaseConfiguration<EmployeePayslipEntity>
{
    public override void Configure(EntityTypeBuilder<EmployeePayslipEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Comment)
            .HasMaxLength(1000);

        builder.HasOne(e => e.Farm)
            .WithMany()
            .HasForeignKey(e => e.FarmId);

        builder.HasOne(e => e.Cycle)
            .WithMany()
            .HasForeignKey(e => e.CycleId);

        builder.HasOne(e => e.Employee)
            .WithMany()
            .HasForeignKey(e => e.EmployeeId);
    }
}