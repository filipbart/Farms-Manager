using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Insertions;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Commands.Insertions;

public record SendToIrzCommand(Guid InsertionId) : IRequest<EmptyBaseResponse>;

public class SendToIrzCommandHandler : IRequestHandler<SendToIrzCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IInsertionRepository _insertionRepository;

    public SendToIrzCommandHandler(IUserDataResolver userDataResolver, IInsertionRepository insertionRepository,
        IUserRepository userRepository)
    {
        _userDataResolver = userDataResolver;
        _insertionRepository = insertionRepository;
        _userRepository = userRepository;
    }

    public async Task<EmptyBaseResponse> Handle(SendToIrzCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), ct) ??
                   throw DomainException.UserNotFound();

        var insertion =
            await _insertionRepository.GetAsync(new InsertionByIdSpec(request.InsertionId), ct);

        if (insertion.DateIrzSentUtc.HasValue || insertion.IsSentToIrz)
        {
            response.AddError("Insertion", "Wstawienie zostało już wysłane do systemu IRZ+");
            return response;
        }

        if (user.IrzplusCredentials is null || user.IrzplusCredentials.EncryptedPassword.IsEmpty() ||
            user.IrzplusCredentials.Login.IsEmpty())
        {
            response.AddError("IrzplusCredentials", "Brak danych logowania do systemu IRZ+");
            return response;
        }


        
        //insertion.MarkAsSentToIrz(userId);
        await _insertionRepository.UpdateAsync(insertion, ct);

        return response;
    }
}