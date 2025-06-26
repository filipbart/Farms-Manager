using FarmsManager.Application.Common;
using FarmsManager.Application.Models.Irzplus.Common;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.UserAggregate.Models;

namespace FarmsManager.Application.Interfaces;

public interface IIrzplusService : IService
{
    public void PrepareOptions(IrzplusCredentials credentials);
    public Task<ZlozenieDyspozycjiResponse> SendInsertionsAsync(IList<InsertionEntity> insertions, CancellationToken ct = default);
}