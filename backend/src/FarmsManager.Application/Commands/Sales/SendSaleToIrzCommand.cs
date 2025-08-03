using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Sales;

public record SendSaleToIrzCommand(Guid? SaleId, Guid? InternalGroupId) : IRequest<EmptyBaseResponse>;

public class SendSaleToIrzCommandHandler : IRequestHandler<SendSaleToIrzCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IIrzplusService _irzplusService;

    public SendSaleToIrzCommandHandler(IUserDataResolver userDataResolver, ISaleRepository saleRepository,
        IUserRepository userRepository, IIrzplusService irzplusService)
    {
        _userDataResolver = userDataResolver;
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _irzplusService = irzplusService;
    }

    public async Task<EmptyBaseResponse> Handle(SendSaleToIrzCommand request, CancellationToken ct)
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

        List<SaleEntity> salesToSend = [];
        if (request.SaleId.HasValue)
        {
            var sale =
                await _saleRepository.GetAsync(new SaleByIdSpec(request.SaleId.Value), ct);

            if (sale.IsAlreadySentToIrz())
            {
                response.AddError("Sale", "Wstawienie zostało już wysłane do systemu IRZplus");
                return response;
            }

            salesToSend.Add(sale);
        }

        else if (request.InternalGroupId.HasValue)
        {
            var sales =
                await _saleRepository.ListAsync(
                    new GetSalesToSendByInternalGroupId(request.InternalGroupId.Value), ct);
            if (sales.Count == 0)
            {
                response.AddError("Sales", "Brak wstawień do wysłania");
                return response;
            }

            salesToSend.AddRange(sales);
        }
        else
        {
            response.AddError("Sales", "Brak wstawień do wysłania");
            return response;
        }

        var hasDifferentFarms = salesToSend.Select(x => x.FarmId).Distinct().Skip(1).Any();
        var hasDifferentCycles = salesToSend.Select(x => x.CycleId).Distinct().Skip(1).Any();

        if (hasDifferentFarms || hasDifferentCycles)
        {
            response.AddError("InvalidData", "Wstawienia muszą dotyczyć jednej farmy i jednego cyklu");
            return response;
        }


        var dispositionResponse = await _irzplusService.SendSalesAsync(salesToSend, ct);
        if (dispositionResponse.Bledy.Count != 0)
        {
            foreach (var bladWalidacjiDto in dispositionResponse.Bledy)
            {
                response.AddError(bladWalidacjiDto.KodBledu, bladWalidacjiDto.Komunikat);
            }

            return response;
        }

        if (dispositionResponse.NumerDokumentu.IsEmpty())
        {
            throw new Exception("Numer dokumentu z systemu IRZplus jest pusty");
        }

        salesToSend.ForEach(t => t.MarkAsSentToIrz(dispositionResponse.NumerDokumentu, userId));
        await _saleRepository.UpdateRangeAsync(salesToSend, ct);

        return response;
    }
}

public sealed class GetSalesToSendByInternalGroupId : BaseSpecification<SaleEntity>
{
    public GetSalesToSendByInternalGroupId(Guid internalGroupId)
    {
        EnsureExists();
        Query.Where(t => t.InternalGroupId == internalGroupId);
        Query.Where(t => t.DateIrzSentUtc.HasValue == false);
        Query.Where(t => t.IsSentToIrz == false);
        Query.Include(t => t.Farm);
        Query.Include(t => t.Slaughterhouse);
        Query.Include(t => t.Cycle);
        Query.Include(t => t.Henhouse);
    }
}

public class SendSaleToIrzCommandValidator : AbstractValidator<SendSaleToIrzCommand>
{
    public SendSaleToIrzCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.SaleId.HasValue || x.InternalGroupId.HasValue)
            .WithMessage("Musi być podane 'SaleId' lub 'InternalGroupId'");
    }
}