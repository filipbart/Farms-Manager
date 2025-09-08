using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.UserAggregate;

public class UserEntityConfiguration : BaseConfiguration<UserEntity>
{
    public override void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Login);
        builder.Property(t => t.IrzplusCredentials).IsRequired(false).HasColumnType("jsonb");
        builder.Property(t => t.IsAdmin).IsRequired().HasDefaultValue(false);
        builder.Property(t => t.MustChangePassword).IsRequired().HasDefaultValue(true);
    }
}