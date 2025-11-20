using FarmsManager.Application.Commands.Slaughterhouses;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Entities;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Sales.Invoices;

public record SaveSalesInvoiceCommand : IRequest<EmptyBaseResponse>
{
    public string FilePath { get; init; }
    public Guid DraftId { get; init; }
    public AddSaleInvoiceDto Data { get; init; }
}

public class SaveSalesInvoiceCommandHandler : IRequestHandler<SaveSalesInvoiceCommand, EmptyBaseResponse>
{
    private readonly IS3Service _s3Service;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;

    public SaveSalesInvoiceCommandHandler(IS3Service s3Service, IFarmRepository farmRepository,
        ICycleRepository cycleRepository, IUserDataResolver userDataResolver,
        ISlaughterhouseRepository slaughterhouseRepository,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _s3Service = s3Service;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _userDataResolver = userDataResolver;
        _slaughterhouseRepository = slaughterhouseRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<EmptyBaseResponse> Handle(SaveSalesInvoiceCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId!.Value), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId!.Value), ct);
        var slaughterhouse =
            await _slaughterhouseRepository.GetAsync(
                new SlaughterhouseByIdSpec(request.Data.SlaughterhouseId!.Value), ct);

        if (await _s3Service.FileExistsAsync(FileType.SalesInvoices, request.FilePath) == false)
        {
            response.AddError("FileUrl", "Nie znaleziono pliku");
            return response;
        }

        var existedInvoice = await _saleInvoiceRepository.FirstOrDefaultAsync(
            new GetSaleInvoiceByInvoiceNumberAndFarmSpec(request.Data.InvoiceNumber, farm.Id), ct);
        if (existedInvoice is not null)
        {
            throw new Exception($"Istnieje już faktura sprzedaży z numerem '{existedInvoice.InvoiceNumber}' dla fermy '{farm.Name}'.");
        }

        var newSalesInvoice = SaleInvoiceEntity.CreateNew(
            farm.Id,
            cycle.Id,
            slaughterhouse.Id,
            request.Data.InvoiceNumber!,
            request.Data.InvoiceDate!.Value,
            request.Data.DueDate!.Value,
            request.Data.InvoiceTotal!.Value,
            request.Data.SubTotal!.Value,
            request.Data.VatAmount!.Value,
            userId);

        var newPath = request.FilePath.Replace(request.DraftId.ToString(), newSalesInvoice.Id.ToString())
            .Replace("draft", "saved");
        newSalesInvoice.SetFilePath(newPath);

        await _saleInvoiceRepository.AddAsync(newSalesInvoice, ct);

        await _s3Service.MoveFileAsync(FileType.SalesInvoices, request.FilePath, newPath);

        return response;
    }
}

public class SaveSalesInvoiceCommandValidator : AbstractValidator<SaveSalesInvoiceCommand>
{
    public SaveSalesInvoiceCommandValidator()
    {
        RuleFor(x => x.FilePath).NotNull().NotEmpty();
        RuleFor(x => x.DraftId).NotEmpty();
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.FarmId).NotEmpty();
        RuleFor(x => x.Data.CycleId).NotEmpty();
        RuleFor(x => x.Data.SlaughterhouseId).NotEmpty();
        RuleFor(x => x.Data.InvoiceNumber).NotEmpty();
        RuleFor(x => x.Data.InvoiceDate).NotEmpty();
        RuleFor(x => x.Data.DueDate).NotEmpty();
        RuleFor(x => x.Data.InvoiceTotal).GreaterThan(0);
        RuleFor(x => x.Data.SubTotal).GreaterThan(0);
        RuleFor(x => x.Data.VatAmount).GreaterThan(0);
    }
}