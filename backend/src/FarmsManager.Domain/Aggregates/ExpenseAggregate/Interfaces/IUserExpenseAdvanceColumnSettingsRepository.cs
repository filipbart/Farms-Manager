using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;

public interface IUserExpenseAdvanceColumnSettingsRepository : IRepository<UserExpenseAdvanceColumnSettingsEntity>
{
    Task<UserExpenseAdvanceColumnSettingsEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
