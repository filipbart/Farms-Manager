using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Insertions;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Insertions;

public record DeleteInsertionCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteInsertionCommandHandler : IRequestHandler<DeleteInsertionCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IInsertionRepository _insertionRepository;

    public DeleteInsertionCommandHandler(IUserDataResolver userDataResolver, IInsertionRepository insertionRepository)
    {
        _userDataResolver = userDataResolver;
        _insertionRepository = insertionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteInsertionCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var insertion = await _insertionRepository.GetAsync(new InsertionByIdSpec(request.Id), cancellationToken);

        insertion.Delete(userId);
        await _insertionRepository.UpdateAsync(insertion, cancellationToken);

        return new EmptyBaseResponse();
    }
}