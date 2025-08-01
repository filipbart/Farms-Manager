using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.HatcheryAggregate;

public class HatcheryNoteEntityConfiguration : BaseConfiguration<HatcheryNoteEntity>
{
    public override void Configure(EntityTypeBuilder<HatcheryNoteEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);
    }
}