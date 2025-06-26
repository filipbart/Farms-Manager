using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Insertions;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Insertions;

public record SendToIrzCommand(Guid? InsertionId, Guid? InternalGroupId) : IRequest<EmptyBaseResponse>;

public class SendToIrzCommandHandler : IRequestHandler<SendToIrzCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IInsertionRepository _insertionRepository;
    private readonly IIrzplusService _irzplusService;

    public SendToIrzCommandHandler(IUserDataResolver userDataResolver, IInsertionRepository insertionRepository,
        IUserRepository userRepository, IIrzplusService irzplusService)
    {
        _userDataResolver = userDataResolver;
        _insertionRepository = insertionRepository;
        _userRepository = userRepository;
        _irzplusService = irzplusService;
    }

    public async Task<EmptyBaseResponse> Handle(SendToIrzCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();

        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), ct) ??
                   throw DomainException.UserNotFound();

        if (user.IrzplusCredentials is null || user.IrzplusCredentials.EncryptedPassword.IsEmpty() ||
            user.IrzplusCredentials.Login.IsEmpty())
        {
            response.AddError("IrzplusCredentials", "Brak danych logowania do systemu IRZplus");
            return response;
        }

        _irzplusService.PrepareOptions(user.IrzplusCredentials);

        List<InsertionEntity> insertionsToSend = [];
        if (request.InsertionId.HasValue)
        {
            var insertion =
                await _insertionRepository.GetAsync(new InsertionByIdSpec(request.InsertionId.Value), ct);

            if (insertion.IsAlreadySentToIrz())
            {
                response.AddError("Insertion", "Wstawienie zostało już wysłane do systemu IRZplus");
                return response;
            }

            insertionsToSend.Add(insertion);
        }

        else if (request.InternalGroupId.HasValue)
        {
            var insertions =
                await _insertionRepository.ListAsync(
                    new GetInsertionsToSendByInternalGroupId(request.InternalGroupId.Value), ct);
            if (insertions.Count == 0)
            {
                response.AddError("Insertions", "Brak wstawień do wysłania");
                return response;
            }

            insertionsToSend.AddRange(insertions);
        }
        else
        {
            response.AddError("Insertions", "Brak wstawień do wysłania");
            return response;
        }

        var hasDifferentFarms = insertionsToSend.Select(x => x.FarmId).Distinct().Skip(1).Any();
        var hasDifferentCycles = insertionsToSend.Select(x => x.CycleId).Distinct().Skip(1).Any();

        if (hasDifferentFarms || hasDifferentCycles)
        {
            response.AddError("InvalidData", "Wstawienia muszą dotyczyć jednej farmy i jednego cyklu");
            return response;
        }


        var dispositionResponse = await _irzplusService.SendInsertionsAsync(insertionsToSend, ct);
        if (dispositionResponse.Bledy.Count != 0)
        {
            foreach (var bladWalidacjiDto in dispositionResponse.Bledy)
            {
                response.AddError(bladWalidacjiDto.KodBledu, bladWalidacjiDto.Komunikat);
            }

            return response;
        }

        insertionsToSend.ForEach(t => t.MarkAsSentToIrz(userId));
        await _insertionRepository.UpdateRangeAsync(insertionsToSend, ct);


        return response;
    }
}

public sealed class GetInsertionsToSendByInternalGroupId : BaseSpecification<InsertionEntity>
{
    public GetInsertionsToSendByInternalGroupId(Guid internalGroupId)
    {
        EnsureExists();
        Query.Where(t => t.InternalGroupId == internalGroupId);
        Query.Where(t => t.DateIrzSentUtc.HasValue == false);
        Query.Where(t => t.IsSentToIrz == false);
        Query.Include(t => t.Farm);
        Query.Include(t => t.Hatchery);
        Query.Include(t => t.Cycle);
        Query.Include(t => t.Henhouse);
    }
}

public class SendToIrzCommandValidator : AbstractValidator<SendToIrzCommand>
{
    public SendToIrzCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.InsertionId.HasValue || x.InternalGroupId.HasValue)
            .WithMessage("Musi być podane 'InsertionId' lub 'InternalGroupId'");
    }
}