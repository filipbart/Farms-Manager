using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.TaxBusinessEntities;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.TaxBusinessEntities;

public record UpdateTaxBusinessEntityCommand(Guid Id, UpdateTaxBusinessEntityDto Data) : IRequest<EmptyBaseResponse>;

public record UpdateTaxBusinessEntityDto
{
    public string Name { get; init; }
    public string BusinessType { get; init; }
    public string Description { get; init; }
    public string KSeFToken { get; init; }
}

public class UpdateTaxBusinessEntityDtoValidator : AbstractValidator<UpdateTaxBusinessEntityDto>
{
    public UpdateTaxBusinessEntityDtoValidator()
    {
        RuleFor(t => t.Name).NotEmpty().WithMessage("Nazwa jest wymagana.");
        RuleFor(t => t.BusinessType).NotEmpty().WithMessage("Typ działalności jest wymagany.");
    }
}

public class UpdateTaxBusinessEntityCommandHandler : IRequestHandler<UpdateTaxBusinessEntityCommand, EmptyBaseResponse>
{
    private readonly ITaxBusinessEntityRepository _repository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IEncryptionService _encryptionService;

    public UpdateTaxBusinessEntityCommandHandler(
        ITaxBusinessEntityRepository repository, 
        IUserDataResolver userDataResolver,
        IEncryptionService encryptionService)
    {
        _repository = repository;
        _userDataResolver = userDataResolver;
        _encryptionService = encryptionService;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateTaxBusinessEntityCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var entity = await _repository.GetAsync(new TaxBusinessEntityByIdSpec(request.Id), cancellationToken)
            ?? throw DomainException.RecordNotFound("Podmiot gospodarczy");

        var encryptedToken = !string.IsNullOrWhiteSpace(request.Data.KSeFToken) 
            ? _encryptionService.Encrypt(request.Data.KSeFToken) 
            : null;

        entity.Update(request.Data.Name, request.Data.BusinessType, request.Data.Description, encryptedToken);
        entity.SetModified(userId);
        
        await _repository.UpdateAsync(entity, cancellationToken);

        return new EmptyBaseResponse();
    }
}
