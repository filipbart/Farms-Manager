using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.UserAggregate;

public class UserSessionEntityConfiguration : BaseConfiguration<UserSessionEntity>
{
    public override void Configure(EntityTypeBuilder<UserSessionEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.SessionId).IsUnique();
    }
}