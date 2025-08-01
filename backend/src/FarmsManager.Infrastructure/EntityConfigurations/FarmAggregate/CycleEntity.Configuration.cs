using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FarmAggregate;

public class CycleEntityConfiguration : BaseConfiguration<CycleEntity>
{
    public override void Configure(EntityTypeBuilder<CycleEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => new { t.Identifier, t.Year, t.FarmId }).IsUnique().HasFilter("date_deleted_utc IS NULL");
    }
}