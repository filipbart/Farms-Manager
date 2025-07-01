using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.SalesSettings;

public record DeleteSaleFieldExtraCommand(Guid SaleFieldExtraId) : IRequest<EmptyBaseResponse>;

public class DeleteSaleFieldExtraCommandHandler : IRequestHandler<DeleteSaleFieldExtraCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleFieldExtraRepository _saleFieldExtraRepository;

    public DeleteSaleFieldExtraCommandHandler(
        IUserDataResolver userDataResolver,
        ISaleFieldExtraRepository saleFieldExtraRepository)
    {
        _userDataResolver = userDataResolver;
        _saleFieldExtraRepository = saleFieldExtraRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteSaleFieldExtraCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity = await _saleFieldExtraRepository.GetAsync(new GetSaleFieldExtraByIdSpec(request.SaleFieldExtraId),
            cancellationToken);

        entity.Delete(userId);
        await _saleFieldExtraRepository.UpdateAsync(entity, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public sealed class GetSaleFieldExtraByIdSpec : BaseSpecification<SaleFieldExtraEntity>,
    ISingleResultSpecification<SaleFieldExtraEntity>
{
    public GetSaleFieldExtraByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}