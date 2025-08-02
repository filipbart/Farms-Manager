using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record HatcheryNoteData
{
    public string Title { get; init; }
    public string Content { get; init; }
}

public record AddHatcheryNoteCommand(HatcheryNoteData Data) : IRequest<EmptyBaseResponse>;

public class AddHatcheryNoteCommandHandler : IRequestHandler<AddHatcheryNoteCommand, EmptyBaseResponse>
{
    private readonly IHatcheryNoteRepository _hatcheryNoteRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddHatcheryNoteCommandHandler(IHatcheryNoteRepository hatcheryNoteRepository,
        IUserDataResolver userDataResolver)
    {
        _hatcheryNoteRepository = hatcheryNoteRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddHatcheryNoteCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var newNote = HatcheryNoteEntity.CreateNew(
            request.Data.Title,
            request.Data.Content,
            userId
        );

        await _hatcheryNoteRepository.AddAsync(newNote, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}