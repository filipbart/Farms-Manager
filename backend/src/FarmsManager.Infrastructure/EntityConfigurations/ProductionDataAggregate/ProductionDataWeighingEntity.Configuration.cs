﻿using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.ProductionDataAggregate;

public class ProductionDataWeighingEntityConfiguration : BaseConfiguration<ProductionDataWeighingEntity>
{
    public override void Configure(EntityTypeBuilder<ProductionDataWeighingEntity> builder)
    {
        base.Configure(builder);

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => new { t.CycleId, t.FarmId, t.HenhouseId, t.HatcheryId }).IsUnique()
            .HasFilter("date_deleted_utc IS NULL");

        builder.HasOne(t => t.Hatchery).WithMany().HasForeignKey(t => t.HatcheryId);
        builder.HasOne(t => t.Henhouse).WithMany().HasForeignKey(t => t.HenhouseId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
    }
}