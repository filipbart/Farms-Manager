using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Insertions;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Insertions;

public record UpdateInsertionCommandDto
{
    /// <summary>
    /// Ilość sztuk
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Data wstawienia
    /// </summary>
    public DateOnly InsertionDate { get; init; }

    /// <summary>
    /// Średnia masa ciała
    /// </summary>
    public decimal BodyWeight { get; init; }
}

public record UpdateInsertionCommand(Guid Id, UpdateInsertionCommandDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateInsertionCommandHandler : IRequestHandler<UpdateInsertionCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IInsertionRepository _insertionRepository;

    public UpdateInsertionCommandHandler(IUserDataResolver userDataResolver, IInsertionRepository insertionRepository)
    {
        _userDataResolver = userDataResolver;
        _insertionRepository = insertionRepository;
    }


    public async Task<EmptyBaseResponse> Handle(UpdateInsertionCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var insertion = await _insertionRepository.GetAsync(new InsertionByIdSpec(request.Id), cancellationToken);

        if (request.Data.Quantity == insertion.Quantity && request.Data.BodyWeight == insertion.BodyWeight &&
            request.Data.InsertionDate == insertion.InsertionDate) return new EmptyBaseResponse();

        insertion.UpdateData(request.Data.InsertionDate, request.Data.Quantity, request.Data.BodyWeight);
        insertion.SetModified(userId);
        await _insertionRepository.UpdateAsync(insertion, cancellationToken);

        return new EmptyBaseResponse();
    }
}