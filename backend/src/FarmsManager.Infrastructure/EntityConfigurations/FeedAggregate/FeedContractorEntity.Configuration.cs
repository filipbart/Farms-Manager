using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FeedAggregate;

public class FeedContractorEntityConfiguration : BaseConfiguration<FeedContractorEntity>
{
    public override void Configure(EntityTypeBuilder<FeedContractorEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);
    }
}