﻿using Ardalis.Specification;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entites;

namespace FarmsManager.Application.Specifications.Feeds;

public sealed class GetFeedInvoiceByIdSpec : BaseSpecification<FeedInvoiceEntity>,
    ISingleResultSpecification<FeedInvoiceEntity>
{
    public GetFeedInvoiceByIdSpec(Guid id)
    {
        EnsureExists();

        Query.Where(t => t.Id == id);
    }
}