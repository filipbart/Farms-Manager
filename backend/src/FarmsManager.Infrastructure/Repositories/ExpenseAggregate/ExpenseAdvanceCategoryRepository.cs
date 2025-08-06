using AutoMapper;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Infrastructure.Repositories.ExpenseAggregate;

public class ExpenseAdvanceCategoryRepository : AbstractRepository<ExpenseAdvanceCategoryEntity>,
    IExpenseAdvanceCategoryRepository
{
    private readonly FarmsManagerContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public ExpenseAdvanceCategoryRepository(FarmsManagerContext context, IConfigurationProvider configurationProvider) :
        base(context,
            configurationProvider)
    {
        _context = context;
    }
}