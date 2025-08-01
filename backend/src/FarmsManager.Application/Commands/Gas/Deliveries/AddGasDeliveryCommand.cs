using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Gas.Deliveries;

public record AddGasDeliveryData
{
    public Guid FarmId { get; init; }
    public Guid ContractorId { get; init; }
    public string InvoiceNumber { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Quantity { get; init; }
    public string Comment { get; init; }
    public IFormFile File { get; init; }
}

public record AddGasDeliveryCommand(AddGasDeliveryData Data) : IRequest<EmptyBaseResponse>;

public class AddGasDeliveryCommandHandler : IRequestHandler<AddGasDeliveryCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IGasContractorRepository _gasContractorRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;

    public AddGasDeliveryCommandHandler(IS3Service s3Service, IFarmRepository farmRepository,
        IUserDataResolver userDataResolver, IGasContractorRepository gasContractorRepository,
        IGasDeliveryRepository gasDeliveryRepository)
    {
        _s3Service = s3Service;
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
        _gasContractorRepository = gasContractorRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddGasDeliveryCommand request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId), ct);
        var contractor =
            await _gasContractorRepository.GetAsync(new GetGasContractorByIdSpec(request.Data.ContractorId), ct);

        var newGasDelivery = GasDeliveryEntity.CreateNew(
            farm.Id,
            contractor.Id,
            request.Data.InvoiceNumber,
            request.Data.InvoiceDate,
            request.Data.UnitPrice * request.Data.Quantity,
            request.Data.UnitPrice,
            request.Data.Quantity,
            request.Data.Comment,
            userId);

        if (request.Data.File != null)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(request.Data.File.FileName);
            var filePath = "saved/" + fileId + extension;

            using var memoryStream = new MemoryStream();
            await request.Data.File.CopyToAsync(memoryStream, ct);
            var fileBytes = memoryStream.ToArray();

            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.GasDelivery, filePath);
            newGasDelivery.SetFilePath(key);
        }

        await _gasDeliveryRepository.AddAsync(newGasDelivery, ct);
        return new EmptyBaseResponse();
    }
}

public class AddGasDeliveryCommandValidator : AbstractValidator<AddGasDeliveryCommand>
{
    public AddGasDeliveryCommandValidator()
    {
        RuleFor(t => t.Data.FarmId).NotEmpty();
        RuleFor(t => t.Data.ContractorId).NotEmpty();
        RuleFor(t => t.Data.InvoiceNumber).NotEmpty();
        RuleFor(t => t.Data.UnitPrice).GreaterThan(0);
        RuleFor(t => t.Data.Quantity).GreaterThan(0);
    }
}