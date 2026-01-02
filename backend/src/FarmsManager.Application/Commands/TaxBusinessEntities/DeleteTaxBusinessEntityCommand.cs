using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.TaxBusinessEntities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.TaxBusinessEntities;

public record DeleteTaxBusinessEntityCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteTaxBusinessEntityCommandHandler : IRequestHandler<DeleteTaxBusinessEntityCommand, EmptyBaseResponse>
{
    private readonly ITaxBusinessEntityRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public DeleteTaxBusinessEntityCommandHandler(ITaxBusinessEntityRepository repository, IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteTaxBusinessEntityCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity = await _repository.GetAsync(new TaxBusinessEntityByIdSpec(request.Id), cancellationToken)
            ?? throw DomainException.RecordNotFound("Podmiot gospodarczy");

        entity.Delete(userId);
        await _repository.UpdateAsync(entity, cancellationToken);

        return new EmptyBaseResponse();
    }
}
