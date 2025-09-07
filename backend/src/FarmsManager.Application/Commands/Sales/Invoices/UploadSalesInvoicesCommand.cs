using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.AzureDi;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Sales.Invoices;

public record UploadSalesInvoicesDto
{
    public List<IFormFile> Files { get; init; }
}

public record UploadSalesInvoicesData
{
    public Guid DraftId { get; init; }
    public string FileUrl { get; init; }
    public string FilePath { get; init; }
    public AddSaleInvoiceDto ExtractedFields { get; init; }
}

public record UploadSalesInvoicesCommandResponse
{
    public List<UploadSalesInvoicesData> Files { get; set; } = [];
}

public record UploadSalesInvoicesCommand(UploadSalesInvoicesDto Dto)
    : IRequest<BaseResponse<UploadSalesInvoicesCommandResponse>>;

public class UploadSalesInvoicesCommandHandler : IRequestHandler<UploadSalesInvoicesCommand,
    BaseResponse<UploadSalesInvoicesCommandResponse>>
{
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;
    private readonly IAzureDiService _azureDiService;
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly ISaleInvoiceRepository _saleInvoiceRepository;


    public UploadSalesInvoicesCommandHandler(IMapper mapper, IS3Service s3Service, IAzureDiService azureDiService,
        IFarmRepository farmRepository, IUserDataResolver userDataResolver,
        ISlaughterhouseRepository slaughterhouseRepository,
        ISaleInvoiceRepository saleInvoiceRepository)
    {
        _mapper = mapper;
        _s3Service = s3Service;
        _azureDiService = azureDiService;
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
        _slaughterhouseRepository = slaughterhouseRepository;
        _saleInvoiceRepository = saleInvoiceRepository;
    }

    public async Task<BaseResponse<UploadSalesInvoicesCommandResponse>> Handle(UploadSalesInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = BaseResponse.CreateResponse(new UploadSalesInvoicesCommandResponse());

        foreach (var file in request.Dto.Files)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var newFileName = fileId + extension;
            var filePath = "draft/" + fileId + extension;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.SalesInvoices, filePath);

            var preSignedUrl = _s3Service.GeneratePreSignedUrl(FileType.SalesInvoices, filePath, newFileName);

            var salesInvoiceModel = await _azureDiService.AnalyzeInvoiceAsync<SaleInvoiceModel>(preSignedUrl);
            var extractedFields = _mapper.Map<AddSaleInvoiceDto>(salesInvoiceModel);

            var existedInvoice = await _saleInvoiceRepository.FirstOrDefaultAsync(
                new GetSaleInvoiceByInvoiceNumberSpec(extractedFields.InvoiceNumber), cancellationToken);

            if (existedInvoice is not null)
            {
                throw new Exception($"Istnieje już faktura sprzedaży z tym numerem: {existedInvoice.InvoiceNumber}");
            }

            // A rzeźnia to KLIENT (Customer)
            var slaughterhouse = await _slaughterhouseRepository.FirstOrDefaultAsync(
                new SlaughterhouseByNipSpec(salesInvoiceModel.CustomerNip?.Replace("-", "")),
                cancellationToken);

            if (slaughterhouse is null)
            {
                slaughterhouse = SlaughterhouseEntity.CreateNew(salesInvoiceModel.CustomerName, string.Empty,
                    salesInvoiceModel.CustomerNip, salesInvoiceModel.CustomerAddress, userId);
                await _slaughterhouseRepository.AddAsync(slaughterhouse, cancellationToken);
            }

            // W fakturze sprzedaży, nasza ferma to SPRZEDAWCA (Vendor)
            var farm = await _farmRepository.FirstOrDefaultAsync(new FarmByNipOrNameSpec(
                    salesInvoiceModel.VendorNip?.Replace("-", ""),
                    salesInvoiceModel.VendorName),
                cancellationToken);


            extractedFields.FarmId = farm?.Id;
            extractedFields.CycleId = farm?.ActiveCycleId;
            extractedFields.SlaughterhouseId = slaughterhouse?.Id;

            response.ResponseData.Files.Add(new UploadSalesInvoicesData
            {
                DraftId = fileId,
                FilePath = key,
                FileUrl = preSignedUrl,
                ExtractedFields = extractedFields
            });
        }

        return response;
    }
}

public class UploadSalesInvoicesCommandValidator : AbstractValidator<UploadSalesInvoicesCommand>
{
    public UploadSalesInvoicesCommandValidator()
    {
        RuleFor(t => t.Dto.Files).NotEmpty().WithMessage("Lista przesłanych plików jest pusta.");
    }
}

public sealed class SlaughterhouseByNipSpec : BaseSpecification<SlaughterhouseEntity>,
    ISingleResultSpecification<SlaughterhouseEntity>
{
    public SlaughterhouseByNipSpec(string nip)
    {
        nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.Nip == nip);
    }
}

public class UploadSalesInvoicesCommandProfile : Profile
{
    public UploadSalesInvoicesCommandProfile()
    {
        CreateMap<SaleInvoiceModel, AddSaleInvoiceDto>()
            .ForMember(m => m.FarmId, opt => opt.Ignore())
            .ForMember(m => m.CycleId, opt => opt.Ignore())
            .ForMember(m => m.SlaughterhouseId, opt => opt.Ignore());
    }
}