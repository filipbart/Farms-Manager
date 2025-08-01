using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record UpdateHatcheryNoteCommand(Guid Id, HatcheryNoteData Data) : IRequest<EmptyBaseResponse>;

public class UpdateHatcheryNoteCommandHandler : IRequestHandler<UpdateHatcheryNoteCommand, EmptyBaseResponse>
{
    private readonly IHatcheryNoteRepository _hatcheryNoteRepository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateHatcheryNoteCommandHandler(IHatcheryNoteRepository hatcheryNoteRepository,
        IUserDataResolver userDataResolver)
    {
        _hatcheryNoteRepository = hatcheryNoteRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateHatcheryNoteCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var hatcheryNote =
            await _hatcheryNoteRepository.GetAsync(new HatcheryNoteByIdSpec(request.Id), cancellationToken);


        hatcheryNote.Update(
            request.Data.Title,
            request.Data.Content
        );
        hatcheryNote.SetModified(userId);

        await _hatcheryNoteRepository.UpdateAsync(hatcheryNote, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public sealed class HatcheryNoteByIdSpec : BaseSpecification<HatcheryNoteEntity>,
    ISingleResultSpecification<HatcheryNoteEntity>
{
    public HatcheryNoteByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}