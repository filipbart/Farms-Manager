using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.AzureDi;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Gas;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.GasAggregate.Entities;
using FarmsManager.Domain.Aggregates.GasAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Gas;

public record UploadGasDeliveriesInvoicesDto
{
    public List<IFormFile> Files { get; init; }
}

public record UploadGasDeliveriesInvoicesData
{
    public Guid DraftId { get; init; }
    public string FileUrl { get; init; }
    public string FilePath { get; init; }
    public AddGasDeliveryInvoiceDto ExtractedFields { get; init; }
}

public record UploadGasDeliveriesInvoicesCommandResponse
{
    public List<UploadGasDeliveriesInvoicesData> Files { get; set; } = [];
}

public record UploadGasDeliveriesInvoicesCommand(UploadGasDeliveriesInvoicesDto Dto)
    : IRequest<BaseResponse<UploadGasDeliveriesInvoicesCommandResponse>>;

public class UploadGasDeliveriesInvoicesCommandHandler : IRequestHandler<UploadGasDeliveriesInvoicesCommand,
    BaseResponse<UploadGasDeliveriesInvoicesCommandResponse>>
{
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;
    private readonly IAzureDiService _azureDiService;
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IGasContractorRepository _gasContractorRepository;
    private readonly IGasDeliveryRepository _gasDeliveryRepository;


    public UploadGasDeliveriesInvoicesCommandHandler(IMapper mapper, IS3Service s3Service,
        IAzureDiService azureDiService,
        IFarmRepository farmRepository, IUserDataResolver userDataResolver,
        IGasContractorRepository gasContractorRepository,
        IGasDeliveryRepository gasDeliveryRepository)
    {
        _mapper = mapper;
        _s3Service = s3Service;
        _azureDiService = azureDiService;
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
        _gasContractorRepository = gasContractorRepository;
        _gasDeliveryRepository = gasDeliveryRepository;
    }

    public async Task<BaseResponse<UploadGasDeliveriesInvoicesCommandResponse>> Handle(
        UploadGasDeliveriesInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = BaseResponse.CreateResponse(new UploadGasDeliveriesInvoicesCommandResponse());

        foreach (var file in request.Dto.Files)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var filePath = "draft/" + fileId + extension;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.GasDelivery, filePath);

            var preSignedUrl = _s3Service.GeneratePreSignedUrl(FileType.GasDelivery, filePath, file.FileName);

            var gasInvoiceModel = await _azureDiService.AnalyzeInvoiceAsync<GasDeliveryInvoiceModel>(preSignedUrl);
            var extractedFields = _mapper.Map<AddGasDeliveryInvoiceDto>(gasInvoiceModel);

            var existedInvoice = await _gasDeliveryRepository.FirstOrDefaultAsync(
                new GetGasDeliveryByInvoiceNumberSpec(extractedFields.InvoiceNumber), cancellationToken);

            if (existedInvoice is not null)
            {
                throw new Exception($"Istnieje już dostawa z tym numerem faktury: {existedInvoice.InvoiceNumber}");
            }

            var farm = await _farmRepository.FirstOrDefaultAsync(new FarmByNipOrNameSpec(
                    gasInvoiceModel.CustomerNip?.Replace("-", ""),
                    gasInvoiceModel.CustomerName),
                cancellationToken);

            var contractor = await _gasContractorRepository.FirstOrDefaultAsync(
                new GasContractorByNipSpec(gasInvoiceModel.VendorNip?.Replace("-", "")),
                cancellationToken);

            if (contractor is null)
            {
                contractor = GasContractorEntity.CreateNew(
                    gasInvoiceModel.VendorName, gasInvoiceModel.VendorNip,
                    gasInvoiceModel.VendorAddress?.Replace("\n", " "), userId);
                await _gasContractorRepository.AddAsync(contractor, cancellationToken);
            }

            extractedFields.FarmId = farm?.Id;
            extractedFields.ContractorId = contractor.Id;

            response.ResponseData.Files.Add(new UploadGasDeliveriesInvoicesData
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

public class UploadGasDeliveriesInvoicesCommandValidator : AbstractValidator<UploadGasDeliveriesInvoicesCommand>
{
    public UploadGasDeliveriesInvoicesCommandValidator()
    {
        RuleFor(t => t.Dto).NotEmpty().WithMessage("Lista przesłanych plików jest pusta.");
        RuleFor(t => t.Dto.Files).NotEmpty().WithMessage("Lista przesłanych plików jest pusta.");
    }
}

public class UploadGasDeliveriesInvoicesCommandProfile : Profile
{
    public UploadGasDeliveriesInvoicesCommandProfile()
    {
        CreateMap<GasDeliveryInvoiceModel, AddGasDeliveryInvoiceDto>()
            .ForMember(m => m.FarmId, opt => opt.Ignore())
            .ForMember(m => m.ContractorId, opt => opt.Ignore())
            .ForMember(m => m.Comment, opt => opt.Ignore())
            .ForMember(m => m.ContractorName, opt => opt.MapFrom(t => t.VendorName))
            .ForMember(m => m.Quantity, opt => opt.MapFrom(t => t.Items.Any() ? t.Items.Sum(i => i.Quantity) : null))
            .ForMember(m => m.UnitPrice,
                opt => opt.MapFrom(t => t.Items.Count != 0 ? t.Items.FirstOrDefault().UnitPrice : null));
    }
}