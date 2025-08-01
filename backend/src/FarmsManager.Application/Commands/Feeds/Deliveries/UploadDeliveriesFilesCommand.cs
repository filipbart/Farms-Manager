using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.AzureDi;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record UploadDeliveriesFilesCommandDto
{
    public List<IFormFile> Files { get; init; }
}

public record UploadDeliveryFileData
{
    public Guid DraftId { get; init; }
    public string FileUrl { get; init; }
    public string FilePath { get; init; }
    public AddFeedDeliveryInvoiceDto ExtractedFields { get; init; }
}

public record UploadDeliveriesFilesCommandResponse
{
    public List<UploadDeliveryFileData> Files { get; set; } = [];
}

public record UploadDeliveriesFilesCommand(UploadDeliveriesFilesCommandDto Data)
    : IRequest<BaseResponse<UploadDeliveriesFilesCommandResponse>>;

public class UploadDeliveriesFilesCommandHandler : IRequestHandler<UploadDeliveriesFilesCommand,
    BaseResponse<UploadDeliveriesFilesCommandResponse>>
{
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;
    private readonly IAzureDiService _azureDiService;
    private readonly IFarmRepository _farmRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;

    public UploadDeliveriesFilesCommandHandler(IMapper mapper, IS3Service s3Service, IAzureDiService azureDiService,
        IFarmRepository farmRepository, IHenhouseRepository henhouseRepository,
        IFeedInvoiceRepository feedInvoiceRepository)
    {
        _mapper = mapper;
        _s3Service = s3Service;
        _azureDiService = azureDiService;
        _farmRepository = farmRepository;
        _henhouseRepository = henhouseRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
    }

    public async Task<BaseResponse<UploadDeliveriesFilesCommandResponse>> Handle(UploadDeliveriesFilesCommand request,
        CancellationToken cancellationToken)
    {
        var response = new UploadDeliveriesFilesCommandResponse();

        foreach (var file in request.Data.Files)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var filePath = "draft/" + fileId + extension;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.FeedDeliveryInvoice, filePath);

            var preSignedUrl = _s3Service.GeneratePreSignedUrl(FileType.FeedDeliveryInvoice, filePath, file.FileName);

            var feedDeliveryInvoiceModel = await _azureDiService.AnalyzeInvoiceAsync<FeedDeliveryInvoiceModel>(preSignedUrl);
            var extractedFields = _mapper.Map<AddFeedDeliveryInvoiceDto>(feedDeliveryInvoiceModel);

            var existedInvoice = await _feedInvoiceRepository.SingleOrDefaultAsync(
                new GetFeedInvoiceByInvoiceNumberSpec(extractedFields.InvoiceNumber), cancellationToken);

            if (existedInvoice is not null)
            {
                throw new Exception($"Istnieje już dostawa z tym numerem faktury: {existedInvoice.InvoiceNumber}");
            }


            var farm = await _farmRepository.FirstOrDefaultAsync(new FarmByNipOrNameSpec(
                    feedDeliveryInvoiceModel.CustomerNip.Replace("-", ""),
                    feedDeliveryInvoiceModel.CustomerName),
                cancellationToken);
            var henhouse =
                await _henhouseRepository.FirstOrDefaultAsync(
                    new HenhouseByNameSpec(feedDeliveryInvoiceModel.HenhouseName), cancellationToken);

            extractedFields.FarmId = farm?.Id;
            extractedFields.CycleId = farm?.ActiveCycleId;
            extractedFields.HenhouseId = henhouse?.Id;

            response.Files.Add(new UploadDeliveryFileData
            {
                DraftId = fileId,
                FilePath = key,
                FileUrl = preSignedUrl,
                ExtractedFields = extractedFields
            });
        }

        return BaseResponse.CreateResponse(response);
    }
}

public class UploadDeliveriesFilesCommandValidator : AbstractValidator<UploadDeliveriesFilesCommand>
{
    public UploadDeliveriesFilesCommandValidator()
    {
        RuleFor(t => t.Data).NotNull().NotEmpty().WithMessage("Lista przesłanych plików jest pusta.");
    }
}

public class UploadDeliveriesFilesCommandProfile : Profile
{
    public UploadDeliveriesFilesCommandProfile()
    {
        CreateMap<FeedDeliveryInvoiceModel, AddFeedDeliveryInvoiceDto>()
            .ForMember(m => m.FarmId, opt => opt.Ignore())
            .ForMember(m => m.CycleId, opt => opt.Ignore())
            .ForMember(m => m.HenhouseId, opt => opt.Ignore())
            .ForMember(m => m.ItemName,
                opt => opt.MapFrom(t => t.Items.Count != 0 ? t.Items.FirstOrDefault().Name : null))
            .ForMember(m => m.Quantity, opt => opt.MapFrom(t => t.Items.Any() ? t.Items.Sum(i => i.Quantity) : null))
            .ForMember(m => m.UnitPrice,
                opt => opt.MapFrom(t => t.Items.Count != 0 ? t.Items.FirstOrDefault().UnitPrice : null));
    }
}