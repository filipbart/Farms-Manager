using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FeedAggregate;

public class FeedPaymentEntityConfiguration : BaseConfiguration<FeedPaymentEntity>
{
    public override void Configure(EntityTypeBuilder<FeedPaymentEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
    }
}