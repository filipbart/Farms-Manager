using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FeedAggregate;

public class FeedNameEntityConfiguration : BaseConfiguration<FeedNameEntity>
{
    public override void Configure(EntityTypeBuilder<FeedNameEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(50);
    }
}