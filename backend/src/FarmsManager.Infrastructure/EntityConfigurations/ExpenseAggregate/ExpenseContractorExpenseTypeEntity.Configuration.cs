using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ExpenseAggregate;

public class ExpenseContractorExpenseTypeEntityConfiguration : BaseConfiguration<ExpenseContractorExpenseTypeEntity>
{
    public override void Configure(EntityTypeBuilder<ExpenseContractorExpenseTypeEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.ExpenseContractor)
            .WithMany(c => c.ExpenseTypes)
            .HasForeignKey(t => t.ExpenseContractorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.ExpenseType)
            .WithMany()
            .HasForeignKey(t => t.ExpenseTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.ExpenseContractorId, t.ExpenseTypeId }).IsUnique();
    }
}
