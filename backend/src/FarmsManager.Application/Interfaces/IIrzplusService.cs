using FarmsManager.Application.Common;
using FarmsManager.Application.Models.Irzplus.Common;
using FarmsManager.Domain.Aggregates.FallenStockAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Models;

namespace FarmsManager.Application.Interfaces;

public interface IIrzplusService : IService
{
    public void PrepareOptions(IrzplusCredentials credentials);

    public Task<ZlozenieDyspozycjiResponse> SendInsertionsAsync(IList<InsertionEntity> insertions,
        CancellationToken ct = default);

    public Task<ZlozenieDyspozycjiResponse> SendSalesAsync(IList<SaleEntity> sales, CancellationToken ct = default);

    public Task<ZlozenieDyspozycjiResponse> SendFallenStocksAsync(IList<FallenStockEntity> fallenStocks, CancellationToken ct = default);

    public Task<PobieranieZwierzatApiResponse> GetFlockAsync(FarmEntity farmEntity, CancellationToken ct = default);
}