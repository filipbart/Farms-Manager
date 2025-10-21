using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ExpenseAggregate;

public class ExpenseProductionEntityConfiguration : BaseConfiguration<ExpenseProductionEntity>
{
    public override void Configure(EntityTypeBuilder<ExpenseProductionEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.ExpenseContractor).WithMany().HasForeignKey(t => t.ExpenseContractorId);
        builder.HasOne(t => t.Creator).WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        builder.HasOne(t => t.Modifier).WithMany().HasForeignKey(t => t.ModifiedBy).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        builder.HasOne(t => t.Deleter).WithMany().HasForeignKey(t => t.DeletedBy).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);
        builder.Property(t => t.FilePath).IsRequired(false);
    }
}