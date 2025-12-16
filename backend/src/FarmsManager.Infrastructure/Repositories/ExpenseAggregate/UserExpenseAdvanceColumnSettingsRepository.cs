using AutoMapper;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Infrastructure.Repositories.ExpenseAggregate;

public class UserExpenseAdvanceColumnSettingsRepository : AbstractRepository<UserExpenseAdvanceColumnSettingsEntity>,
    IUserExpenseAdvanceColumnSettingsRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public UserExpenseAdvanceColumnSettingsRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) :
        base(context, configurationProvider)
    {
        _context = context;
    }

    public async Task<UserExpenseAdvanceColumnSettingsEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserExpenseAdvanceColumnSettingsEntity>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.DateDeletedUtc == null, cancellationToken);
    }
}
