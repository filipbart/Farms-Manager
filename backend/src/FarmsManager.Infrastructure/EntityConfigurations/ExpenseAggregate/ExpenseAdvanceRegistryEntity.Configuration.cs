using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ExpenseAggregate;

public class ExpenseAdvanceRegistryEntityConfiguration : BaseConfiguration<ExpenseAdvanceRegistryEntity>
{
    public override void Configure(EntityTypeBuilder<ExpenseAdvanceRegistryEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.Description)
            .IsRequired(false);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(t => t.Permissions)
            .WithOne(p => p.ExpenseAdvanceRegistry)
            .HasForeignKey(p => p.ExpenseAdvanceRegistryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
