using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;
using FluentValidation;

namespace FarmsManager.Application.Commands.Insertions;

public record MarkInsertionsAsSentToIrzCommand(List<Guid> InsertionIds, string Comment)
    : IRequest<EmptyBaseResponse>;

public class MarkInsertionsAsSentToIrzCommandValidator : AbstractValidator<MarkInsertionsAsSentToIrzCommand>
{
    public MarkInsertionsAsSentToIrzCommandValidator()
    {
        RuleFor(x => x.InsertionIds)
            .NotEmpty()
            .WithMessage("Należy wybrać co najmniej jedno wstawienie.");
    }
}

public class
    MarkInsertionsAsSentToIrzCommandHandler : IRequestHandler<MarkInsertionsAsSentToIrzCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IInsertionRepository _insertionRepository;

    public MarkInsertionsAsSentToIrzCommandHandler(IUserDataResolver userDataResolver,
        IInsertionRepository insertionRepository)
    {
        _userDataResolver = userDataResolver;
        _insertionRepository = insertionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(MarkInsertionsAsSentToIrzCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var insertions = await _insertionRepository.ListAsync(new GetInsertionsByIdsSpec(request.InsertionIds), ct);

        if (insertions.Count != request.InsertionIds.Count)
        {
            response.AddError("Insertions", "Nie znaleziono wszystkich wstawień o podanych identyfikatorach.");
            return response;
        }

        if (insertions.Any(i => i.IsSentToIrz))
        {
            response.AddError("Insertions", "Co najmniej jedno z wybranych wstawień zostało już zgłoszone do IRZ.");
            return response;
        }

        foreach (var insertion in insertions)
        {
            insertion.MarkAsSentToIrz(null, userId, request.Comment);
            insertion.SetModified(userId);
        }

        await _insertionRepository.UpdateRangeAsync(insertions, ct);

        return response;
    }
}