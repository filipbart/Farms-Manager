using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FarmAggregate;

public class FarmEntityConfiguration : BaseConfiguration<FarmEntity>
{
    public override void Configure(EntityTypeBuilder<FarmEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasMany(t => t.Henhouses).WithOne(t => t.Farm).HasForeignKey(t => t.FarmId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}