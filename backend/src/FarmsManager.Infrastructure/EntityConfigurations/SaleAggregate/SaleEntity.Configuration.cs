using System.Text.Json;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.SaleAggregate;

public class SaleEntityConfiguration : BaseConfiguration<SaleEntity>
{
    public override void Configure(EntityTypeBuilder<SaleEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);
        builder.HasOne(t => t.Henhouse).WithMany().HasForeignKey(t => t.HenhouseId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.Slaughterhouse).WithMany().HasForeignKey(t => t.SlaughterhouseId);
        builder.HasOne(t => t.Creator).WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Modifier).WithMany().HasForeignKey(t => t.ModifiedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Deleter).WithMany().HasForeignKey(t => t.DeletedBy).OnDelete(DeleteBehavior.Restrict);
        builder.Property(t => t.Comment).IsRequired(false);
        builder.Property(t => t.OtherExtras)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<List<SaleOtherExtras>>(v, JsonOptions)
            )
            .IsRequired(false).HasColumnType("jsonb");
    }
}