using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record UpdateHatcheryNotesOrderCommand(List<Guid> OrderedIds) : IRequest<EmptyBaseResponse>;

public class UpdateHatcheryNotesOrderCommandValidator : AbstractValidator<UpdateHatcheryNotesOrderCommand>
{
    public UpdateHatcheryNotesOrderCommandValidator()
    {
        RuleFor(x => x.OrderedIds)
            .NotEmpty().WithMessage("Lista identyfikatorów nie może być pusta.");
    }
}

public class
    UpdateHatcheryNotesOrderCommandHandler : IRequestHandler<UpdateHatcheryNotesOrderCommand, EmptyBaseResponse>
{
    private readonly IHatcheryNoteRepository _hatcheryNoteRepository;

    public UpdateHatcheryNotesOrderCommandHandler(IHatcheryNoteRepository hatcheryNoteRepository)
    {
        _hatcheryNoteRepository = hatcheryNoteRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateHatcheryNotesOrderCommand request,
        CancellationToken cancellationToken)
    {
        var notes = await _hatcheryNoteRepository.ListAsync(cancellationToken);

        if (notes.Count != request.OrderedIds.Count)
        {
            throw new Exception("Liczba notatek w bazie danych nie zgadza się z liczbą przesłanych identyfikatorów.");
        }

        for (var i = 0; i < request.OrderedIds.Count; i++)
        {
            var noteId = request.OrderedIds[i];
            var note = notes.FirstOrDefault(n => n.Id == noteId);
            note?.UpdateOrder(i);
        }

        await _hatcheryNoteRepository.UpdateRangeAsync(notes, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}