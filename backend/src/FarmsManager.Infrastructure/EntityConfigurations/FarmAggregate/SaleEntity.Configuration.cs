using System.Text.Json;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FarmAggregate;

public class SaleEntityConfiguration : BaseConfiguration<SaleEntity>
{
    public override void Configure(EntityTypeBuilder<SaleEntity> builder)
    {
        

        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => new { t.CycleId, t.FarmId, t.HenhouseId }).IsUnique(); //TODO adjust

        builder.HasOne(t => t.Henhouse).WithMany().HasForeignKey(t => t.HenhouseId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.Slaughterhouse).WithMany().HasForeignKey(t => t.SlaughterhouseId);

        builder.Property(t => t.Comment).IsRequired(false);
        builder.Property(t => t.OtherExtras)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<List<SaleOtherExtras>>(v, JsonOptions)
            )
            .IsRequired(false).HasColumnType("jsonb");
    }
}