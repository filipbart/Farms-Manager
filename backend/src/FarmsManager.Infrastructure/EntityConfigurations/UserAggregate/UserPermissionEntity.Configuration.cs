using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.UserAggregate;

public class UserPermissionEntityConfiguration : BaseConfiguration<UserPermissionEntity>
{
    public override void Configure(EntityTypeBuilder<UserPermissionEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.User).WithMany(t => t.Permissions).HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}