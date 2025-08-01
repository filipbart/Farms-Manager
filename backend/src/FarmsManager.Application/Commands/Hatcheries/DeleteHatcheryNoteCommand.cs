using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record DeleteHatcheryNoteCommand(Guid Id) : IRequest<EmptyBaseResponse>;

public class DeleteHatcheryNoteCommandHandler : IRequestHandler<DeleteHatcheryNoteCommand, EmptyBaseResponse>
{
    private readonly IHatcheryNoteRepository _hatcheryNoteRepository;

    public DeleteHatcheryNoteCommandHandler(IHatcheryNoteRepository hatcheryNoteRepository)
    {
        _hatcheryNoteRepository = hatcheryNoteRepository;
    }

    public async Task<EmptyBaseResponse> Handle(DeleteHatcheryNoteCommand request, CancellationToken cancellationToken)
    {
        var hatcheryNote =
            await _hatcheryNoteRepository.GetAsync(new HatcheryNoteByIdSpec(request.Id), cancellationToken);

        await _hatcheryNoteRepository.DeleteAsync(hatcheryNote, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}