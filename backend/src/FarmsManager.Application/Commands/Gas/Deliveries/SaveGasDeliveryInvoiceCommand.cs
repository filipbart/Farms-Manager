using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Gas.Deliveries;

public record SaveGasDeliveryInvoiceCommand : IRequest<EmptyBaseResponse>
{
    public string FilePath { get; init; }
    public Guid DraftId { get; init; }
    public AddGasDeliveryInvoiceDto Data { get; init; }
}

public class
    SaveGasDeliveryInvoiceCommandHandler : IRequestHandler<SaveGasDeliveryInvoiceCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IGasContractorRepository _gasContractorRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;

    public SaveGasDeliveryInvoiceCommandHandler(IS3Service s3Service, IFarmRepository farmRepository,
        IUserDataResolver userDataResolver,
        IGasContractorRepository gasContractorRepository,
        IGasDeliveryRepository gasDeliveryRepository)
    {
        _s3Service = s3Service;
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
        _gasContractorRepository = gasContractorRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
    }

    public async Task<EmptyBaseResponse> Handle(SaveGasDeliveryInvoiceCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId!.Value), ct);
        var contractor =
            await _gasContractorRepository.GetAsync(
                new GetGasContractorByIdSpec(request.Data.ContractorId!.Value), ct);

        if (await _s3Service.FileExistsAsync(FileType.GasDelivery, request.FilePath) == false)
        {
            response.AddError("FileUrl", "Nie znaleziono pliku");
            return response;
        }

        var existedInvoice = await _gasDeliveryRepository.SingleOrDefaultAsync(
            new GetGasDeliveryByInvoiceNumberSpec(request.Data.InvoiceNumber), ct);
        if (existedInvoice is not null)
        {
            throw new Exception($"Istnieje już dostawa z tym numerem faktury: {existedInvoice.InvoiceNumber}");
        }

        var newGasDelivery = GasDeliveryEntity.CreateNew(
            farm.Id,
            contractor.Id,
            request.Data.InvoiceNumber!,
            request.Data.InvoiceDate!.Value,
            request.Data.UnitPrice!.Value * request.Data.Quantity!.Value,
            request.Data.UnitPrice!.Value,
            request.Data.Quantity!.Value,
            request.Data.Comment,
            userId);

        var newPath = request.FilePath.Replace(request.DraftId.ToString(), newGasDelivery.Id.ToString())
            .Replace("draft", "saved");
        newGasDelivery.SetFilePath(newPath);

        await _gasDeliveryRepository.AddAsync(newGasDelivery, ct);

        await _s3Service.MoveFileAsync(FileType.GasDelivery, request.FilePath, newPath);

        return response;
    }
}

public class SaveGasDeliveryInvoiceCommandValidator : AbstractValidator<SaveGasDeliveryInvoiceCommand>
{
    public SaveGasDeliveryInvoiceCommandValidator()
    {
        RuleFor(x => x.FilePath).NotNull().NotEmpty();
        RuleFor(x => x.DraftId).NotEmpty();
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.FarmId).NotEmpty();
        RuleFor(x => x.Data.ContractorId).NotEmpty();
        RuleFor(x => x.Data.InvoiceNumber).NotEmpty();
        RuleFor(x => x.Data.InvoiceDate).NotEmpty();
        RuleFor(x => x.Data.UnitPrice).GreaterThan(0);
        RuleFor(x => x.Data.Quantity).GreaterThan(0);
    }
}