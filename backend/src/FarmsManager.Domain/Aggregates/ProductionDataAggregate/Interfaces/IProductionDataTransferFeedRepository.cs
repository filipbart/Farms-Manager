﻿using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;

public interface IProductionDataTransferFeedRepository : IRepository<ProductionDataTransferFeedEntity>;