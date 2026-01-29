using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FarmsManager.Application.Queries.Expenses.Contractors;

public class GetExpensesContractorsQueryFilters
{
    public string SearchPhrase { get; init; }
    public bool? ShowDeleted { get; init; }
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
    public List<ExpenseTypeSimpleDto> ExpenseTypes { get; init; } = [];
    public string Nip { get; init; }
    public string Address { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public record ExpenseTypeSimpleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}

public class GetExpensesContractorsQueryHandler : IRequestHandler<GetExpensesContractorsQuery,
    BaseResponse<GetExpensesContractorsQueryResponse>>
{
    private readonly IExpenseContractorRepository _expenseContractorRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetExpensesContractorsQueryHandler(IExpenseContractorRepository expenseContractorRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _expenseContractorRepository = expenseContractorRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetExpensesContractorsQueryResponse>> Handle(GetExpensesContractorsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var isAdmin = user.IsAdmin;

        var items = await _expenseContractorRepository.ListAsync<ExpenseContractorRow>(
            new GetAllExpensesContractorsSpec(request.Filters, isAdmin),
            cancellationToken);
        return BaseResponse.CreateResponse(new GetExpensesContractorsQueryResponse
        {
            Contractors = items.ClearAdminData(isAdmin)
        });
    }
}

public sealed class GetAllExpensesContractorsSpec : BaseSpecification<ExpenseContractorEntity>
{
    public GetAllExpensesContractorsSpec(GetExpensesContractorsQueryFilters filters, bool isAdmin)
    {
        EnsureExists(filters.ShowDeleted, isAdmin);
        DisableTracking();
        Query.Include(t => t.Creator);
        Query.Include(t => t.Modifier);
        Query.Include(t => t.Deleter);
        Query.Include(t => t.ExpenseTypes).ThenInclude(et => et.ExpenseType);
        if (filters.SearchPhrase.IsNotEmpty())
        {
            var phrase = $"%{filters.SearchPhrase}%";
            Query.Where(e => EF.Functions.ILike(e.Name, phrase));
        }
    }
}

public class ExpensesContractorsProfile : Profile
{
    public ExpensesContractorsProfile()
    {
        CreateMap<ExpenseContractorEntity, ExpenseContractorRow>()
            .ForMember(m => m.ExpenseTypes, opt => opt.MapFrom(t => t.ExpenseTypes.Select(et => new ExpenseTypeSimpleDto
            {
                Id = et.ExpenseTypeId,
                Name = et.ExpenseType != null ? et.ExpenseType.Name : ""
            }).ToList()))
            .ForMember(m => m.CreatedByName, opt => opt.MapFrom(t => t.Creator != null ? t.Creator.Name : null))
            .ForMember(m => m.ModifiedByName, opt => opt.MapFrom(t => t.Modifier != null ? t.Modifier.Name : null))
            .ForMember(m => m.DeletedByName, opt => opt.MapFrom(t => t.Deleter != null ? t.Deleter.Name : null));
    }
}