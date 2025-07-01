using FluentValidation;
using MediatR;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.SalesSettings;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;

namespace FarmsManager.Application.Commands.SalesSettings;

public record AddSaleFieldExtraCommand : IRequest<EmptyBaseResponse>
{
    public string[] Fields { get; set; }
}

public class AddSaleFieldExtraCommandHandler : IRequestHandler<AddSaleFieldExtraCommand, EmptyBaseResponse>
{
    private readonly ISaleFieldExtraRepository _saleFieldExtraRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddSaleFieldExtraCommandHandler(ISaleFieldExtraRepository saleFieldExtraRepository,
        IUserDataResolver userDataResolver)
    {
        _saleFieldExtraRepository = saleFieldExtraRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddSaleFieldExtraCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = new EmptyBaseResponse();
        var count = await _saleFieldExtraRepository.CountAsync(new GetAllSaleFieldsExtraSpec(), cancellationToken);
        if (count + request.Fields.Length > 20)
        {
            response.AddError("TooManyFields", "Liczba pól nie może być większa niż 20");
            return response;
        }

        var items = request.Fields.Select(field => SaleFieldExtraEntity.CreateNew(field, userId)).ToList();

        await _saleFieldExtraRepository.AddRangeAsync(items, cancellationToken);

        return response;
    }
}

public class AddSaleFieldExtraCommandValidator : AbstractValidator<AddSaleFieldExtraCommand>
{
    public AddSaleFieldExtraCommandValidator()
    {
        RuleFor(x => x.Fields)
            .NotEmpty()
            .WithMessage("Lista pól nie może byc pusta")
            .Must(x => x.Length <= 20)
            .WithMessage("Liczba pól nie może być większa niż 20");
    }
}