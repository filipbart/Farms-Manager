using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ExpenseAggregate;

public class ExpenseContractorEntityConfiguration : BaseConfiguration<ExpenseContractorEntity>
{
    public override void Configure(EntityTypeBuilder<ExpenseContractorEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);
    }
}