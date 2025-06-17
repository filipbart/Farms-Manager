using FarmsManager.Domain.Aggregates.UserAggregate.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations;

public class UserEntityConfiguration : BaseConfiguration<UserEntity>
{
    public override void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);
    }
}