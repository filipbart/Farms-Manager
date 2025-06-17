using FarmsManager.Domain.Aggregates.UserAggregate.Entites;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;

public interface IUserRepository : IRepository<UserEntity>;