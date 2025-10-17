using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.ExpenseAdvancePermissions;

public record GetExpenseAdvanceRegistriesQuery : IRequest<BaseResponse<GetExpenseAdvanceRegistriesQueryResponse>>;

public record GetExpenseAdvanceRegistriesQueryResponse
{
    public List<ExpenseAdvanceRegistryDto> Registries { get; init; }
}

public class GetExpenseAdvanceRegistriesQueryHandler : IRequestHandler<GetExpenseAdvanceRegistriesQuery,
    BaseResponse<GetExpenseAdvanceRegistriesQueryResponse>>
{
    private readonly IExpenseAdvanceRegistryRepository _registryRepository;

    public GetExpenseAdvanceRegistriesQueryHandler(IExpenseAdvanceRegistryRepository registryRepository)
    {
        _registryRepository = registryRepository;
    }

    public async Task<BaseResponse<GetExpenseAdvanceRegistriesQueryResponse>> Handle(
        GetExpenseAdvanceRegistriesQuery request,
        CancellationToken cancellationToken)
    {
        var registries = await _registryRepository.ListAsync(
            new GetActiveExpenseAdvanceRegistriesSpec(),
            cancellationToken);

        var dtos = registries.Select(r => new ExpenseAdvanceRegistryDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description
        }).ToList();

        return BaseResponse.CreateResponse(new GetExpenseAdvanceRegistriesQueryResponse
        {
            Registries = dtos
        });
    }
}

public sealed class GetActiveExpenseAdvanceRegistriesSpec : BaseSpecification<ExpenseAdvanceRegistryEntity>
{
    public GetActiveExpenseAdvanceRegistriesSpec()
    {
        EnsureExists();
        Query.Where(r => r.IsActive);
        Query.OrderBy(r => r.Name);
    }
}
