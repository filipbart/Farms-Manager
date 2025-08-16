using FarmsManager.Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.UserAggregate;

public class UserFarmEntityConfiguration : BaseConfiguration<UserFarmEntity>
{
    public override void Configure(EntityTypeBuilder<UserFarmEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => new { t.UserId, t.FarmId });

        builder.HasOne(t => t.User).WithMany(t => t.Farms).HasForeignKey(t => t.UserId);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);

        builder.Ignore(t => t.Id);
        builder.Ignore(t => t.DateCreatedUtc);
        builder.Ignore(t => t.DateModifiedUtc);
        builder.Ignore(t => t.DateDeletedUtc);
        builder.Ignore(t => t.CreatedBy);
        builder.Ignore(t => t.ModifiedBy);
        builder.Ignore(t => t.DeletedBy);
    }
}