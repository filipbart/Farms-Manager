using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ExpenseAggregate;

public class ExpenseAdvanceEntityConfiguration : BaseConfiguration<ExpenseAdvanceEntity>
{
    public override void Configure(EntityTypeBuilder<ExpenseAdvanceEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);


        builder.HasOne(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.ExpenseAdvanceCategory).WithMany().HasForeignKey(t => t.ExpenseAdvanceCategoryId);

        builder.Property(t => t.Comment).IsRequired(false);
        builder.Property(t => t.FilePath).IsRequired(false);
    }
}