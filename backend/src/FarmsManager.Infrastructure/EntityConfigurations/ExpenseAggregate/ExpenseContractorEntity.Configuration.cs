using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ExpenseAggregate;

public class ExpenseContractorEntityConfiguration : BaseConfiguration<ExpenseContractorEntity>
{
    public override void Configure(EntityTypeBuilder<ExpenseContractorEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);
        builder.HasOne(t => t.ExpenseType).WithMany().HasForeignKey(t => t.ExpenseTypeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.Creator).WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Modifier).WithMany().HasForeignKey(t => t.ModifiedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Deleter).WithMany().HasForeignKey(t => t.DeletedBy).OnDelete(DeleteBehavior.Restrict);
    }
}