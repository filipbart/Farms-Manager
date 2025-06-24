using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FarmAggregate;

public class InsertionEntityConfiguration : BaseConfiguration<InsertionEntity>
{
    public override void Configure(EntityTypeBuilder<InsertionEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => new { t.CycleId, t.FarmId, t.HenhouseId }).IsUnique();

        builder.HasOne(t => t.Henhouse).WithMany().HasForeignKey(t => t.HenhouseId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
    }
}