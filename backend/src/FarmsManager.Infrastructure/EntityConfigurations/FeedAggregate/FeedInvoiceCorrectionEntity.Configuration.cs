﻿using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmsManager.Infrastructure.EntityConfigurations.FeedAggregate;

public class FeedInvoiceCorrectionEntityConfiguration : BaseConfiguration<FeedInvoiceCorrectionEntity>
{
    public override void Configure(EntityTypeBuilder<FeedInvoiceCorrectionEntity> builder)
    {
        base.Configure(builder);
        builder.HasKey(t => t.Id);

        builder.HasOne(t => t.Farm).WithMany().HasForeignKey(t => t.FarmId);
        builder.HasOne(t => t.Cycle).WithMany().HasForeignKey(t => t.CycleId);
    }
}