using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Queries.Expenses.Contractors;

public class GetExpensesContractorsQueryFilters
{
    public string SearchPhrase { get; init; }
}

public record GetExpensesContractorsQuery(GetExpensesContractorsQueryFilters Filters)
    : IRequest<BaseResponse<GetExpensesContractorsQueryResponse>>;

public record GetExpensesContractorsQueryResponse
{
    public List<ExpenseContractorRow> Contractors { get; set; }
}

public record ExpenseContractorRow
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string ExpenseType { get; init; }
    public Guid? ExpenseTypeId { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public class GetExpensesContractorsQueryHandler : IRequestHandler<GetExpensesContractorsQuery,
    BaseResponse<GetExpensesContractorsQueryResponse>>
{
    private readonly IExpenseContractorRepository _expenseContractorRepository;

    public GetExpensesContractorsQueryHandler(IExpenseContractorRepository expenseContractorRepository)
    {
        _expenseContractorRepository = expenseContractorRepository;
    }

    public async Task<BaseResponse<GetExpensesContractorsQueryResponse>> Handle(GetExpensesContractorsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _expenseContractorRepository.ListAsync<ExpenseContractorRow>(
            new GetAllExpensesContractorsSpec(request.Filters.SearchPhrase),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetExpensesContractorsQueryResponse
        {
            Contractors = items
        });
    }
}

public sealed class GetAllExpensesContractorsSpec : BaseSpecification<ExpenseContractorEntity>
{
    public GetAllExpensesContractorsSpec(string searchPhrase)
    {
        EnsureExists();
        DisableTracking();
        if (searchPhrase.IsNotEmpty())
        {
            var phrase = $"%{searchPhrase}%";
            Query.Where(e => EF.Functions.ILike(e.Name, phrase));
        }
    }
}

public class ExpensesContractorsProfile : Profile
{
    public ExpensesContractorsProfile()
    {
        CreateMap<ExpenseContractorEntity, ExpenseContractorRow>()
            .ForMember(m => m.ExpenseType, opt => opt.MapFrom(t => t.ExpenseType != null ? t.ExpenseType.Name : null));
    }
}