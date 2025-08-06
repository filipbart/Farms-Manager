using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ExpenseAggregate;

public class ExpenseAdvanceCategoryEntityConfiguration : BaseConfiguration<ExpenseAdvanceCategoryEntity>
{
    public override void Configure(EntityTypeBuilder<ExpenseAdvanceCategoryEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
    }
}