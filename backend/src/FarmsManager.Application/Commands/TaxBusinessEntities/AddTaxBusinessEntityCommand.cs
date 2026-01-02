using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Common.Validators;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Entities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.TaxBusinessEntities;

public record AddTaxBusinessEntityCommand : IRequest<EmptyBaseResponse>
{
    public string Nip { get; init; }
    public string Name { get; init; }
    public string BusinessType { get; init; }
    public string Description { get; init; }
}

public class AddTaxBusinessEntityCommandValidator : AbstractValidator<AddTaxBusinessEntityCommand>
{
    public AddTaxBusinessEntityCommandValidator()
    {
        RuleFor(t => t.Nip).NotEmpty().WithMessage("NIP jest wymagany.")
            .Must(ValidationHelpers.IsValidNip).WithMessage("Podany numer NIP jest nieprawidłowy.");
        RuleFor(t => t.Name).NotEmpty().WithMessage("Nazwa jest wymagana.");
        RuleFor(t => t.BusinessType).NotEmpty().WithMessage("Typ działalności jest wymagany.");
    }
}

public class AddTaxBusinessEntityCommandHandler : IRequestHandler<AddTaxBusinessEntityCommand, EmptyBaseResponse>
{
    private readonly ITaxBusinessEntityRepository _repository;
    private readonly IUserDataResolver _userDataResolver;

    public AddTaxBusinessEntityCommandHandler(ITaxBusinessEntityRepository repository, IUserDataResolver userDataResolver)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddTaxBusinessEntityCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity = TaxBusinessEntity.CreateNew(
            request.Nip,
            request.Name,
            request.BusinessType,
            request.Description,
            userId);
        
        await _repository.AddAsync(entity, cancellationToken);

        return new EmptyBaseResponse();
    }
}
