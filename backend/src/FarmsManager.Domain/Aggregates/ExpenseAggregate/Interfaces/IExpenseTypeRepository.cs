using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.SeedWork;

namespace FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;

public interface IExpenseTypeRepository : IRepository<ExpenseTypeEntity>;