using Ardalis.Specification;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.TaxBusinessEntities;

public record GetAllTaxBusinessEntitiesQuery : IRequest<BaseResponse<GetAllTaxBusinessEntitiesQueryResponse>>;

public class GetAllTaxBusinessEntitiesQueryResponse : PaginationModel<TaxBusinessEntityRowDto>;

public record TaxBusinessEntityRowDto
{
    public Guid Id { get; init; }
    public string Nip { get; init; }
    public string Name { get; init; }
    public string BusinessType { get; init; }
    public string Description { get; init; }
    public bool HasKSeFToken { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
}

public class GetAllTaxBusinessEntitiesQueryHandler : IRequestHandler<GetAllTaxBusinessEntitiesQuery, BaseResponse<GetAllTaxBusinessEntitiesQueryResponse>>
{
    private readonly ITaxBusinessEntityRepository _repository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetAllTaxBusinessEntitiesQueryHandler(
        ITaxBusinessEntityRepository repository, 
        IUserDataResolver userDataResolver,
        IUserRepository userRepository)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetAllTaxBusinessEntitiesQueryResponse>> Handle(
        GetAllTaxBusinessEntitiesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var isAdmin = user.IsAdmin;

        var items = await _repository.ListAsync<TaxBusinessEntityRowDto>(
            new GetAllTaxBusinessEntitiesSpec(),
            cancellationToken);
        
        return BaseResponse.CreateResponse(new GetAllTaxBusinessEntitiesQueryResponse
        {
            TotalRows = items.Count,
            Items = items.ClearAdminData(isAdmin)
        });
    }
}

public sealed class GetAllTaxBusinessEntitiesSpec : Specification<TaxBusinessEntity>
{
    public GetAllTaxBusinessEntitiesSpec()
    {
        Query.Where(t => t.DateDeletedUtc.HasValue == false);
        Query.OrderBy(t => t.Name);
    }
}
