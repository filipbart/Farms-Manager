using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.AzureDi;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Application.Specifications.Henhouses;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record UploadDeliveriesFilesCommandDto
{
    public List<IFormFile> Files { get; set; } = [];
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
    private readonly IUserDataResolver _userDataResolver;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IFeedContractorRepository _feedContractorRepository;

    public UploadDeliveriesFilesCommandHandler(IMapper mapper, IS3Service s3Service, IAzureDiService azureDiService,
        IFarmRepository farmRepository, IHenhouseRepository henhouseRepository,
        IFeedInvoiceRepository feedInvoiceRepository, IFeedContractorRepository feedContractorRepository,
        IUserDataResolver userDataResolver)
    {
        _mapper = mapper;
        _s3Service = s3Service;
        _azureDiService = azureDiService;
        _farmRepository = farmRepository;
        _henhouseRepository = henhouseRepository;
        _feedInvoiceRepository = feedInvoiceRepository;
        _feedContractorRepository = feedContractorRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<UploadDeliveriesFilesCommandResponse>> Handle(UploadDeliveriesFilesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = new UploadDeliveriesFilesCommandResponse();

        foreach (var file in request.Data.Files)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var newFileName = fileId + extension;
            var filePath = "draft/" + fileId + extension;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.FeedDeliveryInvoice, filePath,
                cancellationToken);

            var preSignedUrl = _s3Service.GeneratePreSignedUrl(FileType.FeedDeliveryInvoice, filePath, newFileName);

            var feedDeliveryInvoiceModel =
                await _azureDiService.AnalyzeInvoiceAsync<FeedDeliveryInvoiceModel>(preSignedUrl, cancellationToken);
            var extractedFields = _mapper.Map<AddFeedDeliveryInvoiceDto>(feedDeliveryInvoiceModel);

            var existedInvoice = await _feedInvoiceRepository.SingleOrDefaultAsync(
                new GetFeedInvoiceByInvoiceNumberSpec(extractedFields.InvoiceNumber), cancellationToken);

            if (existedInvoice is not null)
            {
                throw new Exception($"Istnieje już dostawa z tym numerem faktury: {existedInvoice.InvoiceNumber}");
            }

            FarmEntity farm = null;
            if (!string.IsNullOrWhiteSpace(feedDeliveryInvoiceModel.CustomerNip))
            {
                farm = await _farmRepository.FirstOrDefaultAsync(
                    new FarmByNipSpec(feedDeliveryInvoiceModel.CustomerNip?.Replace("-", "")),
                    cancellationToken);
            }

            if (farm is null && !string.IsNullOrWhiteSpace(feedDeliveryInvoiceModel.CustomerName))
            {
                farm = await _farmRepository.FirstOrDefaultAsync(
                    new FarmByNameSpec(feedDeliveryInvoiceModel.CustomerName),
                    cancellationToken);
            }

            var henhouse =
                await _henhouseRepository.FirstOrDefaultAsync(
                    new HenhouseByNameAndFarmIdSpec(feedDeliveryInvoiceModel.HenhouseName, farm?.Id),
                    cancellationToken);

            var feedContractor = await _feedContractorRepository.FirstOrDefaultAsync(
                new FeedContractorByNipSpec(feedDeliveryInvoiceModel.VendorNip.Replace("-", "")),
                cancellationToken);

            if (feedContractor is null)
            {
                feedContractor = FeedContractorEntity.CreateNewFromInvoice(
                    feedDeliveryInvoiceModel.VendorName, feedDeliveryInvoiceModel.VendorNip, userId);
                await _feedContractorRepository.AddAsync(feedContractor, cancellationToken);
            }

            extractedFields.FarmId = farm?.Id;
            extractedFields.CycleId = farm?.ActiveCycleId;
            extractedFields.HenhouseId = henhouse?.Id;
            extractedFields.VendorName = feedContractor.Name;

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
            .ForMember(m => m.Quantity,
                opt => opt.MapFrom(t => t.Items.Count != 0 ? t.Items.Sum(i => i.Quantity) : null))
            .ForMember(m => m.UnitPrice,
                opt => opt.MapFrom(t => t.Items.Count != 0 ? t.Items.FirstOrDefault().UnitPrice : null))
            .ForMember(m => m.SubTotal,
                opt => opt.MapFrom(t =>
                    t.Items.Count != 0
                        ? (decimal?)Math.Round(t.Items.Sum(i => i.Quantity * i.UnitPrice).Value, 2,
                            MidpointRounding.AwayFromZero)
                        : null))
            .ForMember(m => m.VatAmount,
                opt => opt.MapFrom(t =>
                    t.Items.Count != 0
                        ? (decimal?)Math.Round(t.Items.Sum(i => i.Quantity * i.UnitPrice * 0.08m).Value, 2,
                            MidpointRounding.AwayFromZero)
                        : null))
            .ForMember(m => m.InvoiceTotal,
                opt => opt.MapFrom(t =>
                    t.Items.Count != 0
                        ? (decimal?)Math.Round(t.Items.Sum(i => i.Quantity * i.UnitPrice * 1.08m).Value, 2,
                            MidpointRounding.AwayFromZero)
                        : null));
    }
}

public sealed class FeedContractorByNipSpec : BaseSpecification<FeedContractorEntity>,
    ISingleResultSpecification<FeedContractorEntity>
{
    public FeedContractorByNipSpec(string nip)
    {
        nip = nip.Replace("PL", "").Replace("-", "").Replace(" ", "").Trim();
        EnsureExists();
        DisableTracking();

        Query.Where(t => t.Nip == nip);
    }
}