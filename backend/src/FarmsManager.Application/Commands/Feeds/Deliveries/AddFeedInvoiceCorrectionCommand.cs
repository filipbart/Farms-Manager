using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Application.Specifications.Feeds;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Entities;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record AddFeedInvoiceCorrectionCommandDto
{
    public string InvoiceNumber { get; init; }
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public decimal SubTotal { get; init; }
    public decimal VatAmount { get; init; }
    public decimal InvoiceTotal { get; init; }
    public DateOnly InvoiceDate { get; init; }
    public IFormFile File { get; init; }
    public List<Guid> FeedInvoiceIds { get; init; }
}

public record AddFeedInvoiceCorrectionCommand(AddFeedInvoiceCorrectionCommandDto Data) : IRequest<EmptyBaseResponse>;

public class
    AddFeedInvoiceCorrectionCommandHandler : IRequestHandler<AddFeedInvoiceCorrectionCommand, EmptyBaseResponse>
{
    public AddFeedInvoiceCorrectionCommandHandler(IS3Service s3Service, IFarmRepository farmRepository,
        ICycleRepository cycleRepository, IUserDataResolver userDataResolver,
        IFeedInvoiceRepository feedInvoiceRepository, IFeedInvoiceCorrectionRepository feedInvoiceCorrectionRepository)
    {
        _s3Service = s3Service;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _userDataResolver = userDataResolver;
        _feedInvoiceRepository = feedInvoiceRepository;
        _feedInvoiceCorrectionRepository = feedInvoiceCorrectionRepository;
    }

    private const string Comment = "Korekta została ujęta: {0}";

    private readonly IS3Service _s3Service;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedInvoiceRepository _feedInvoiceRepository;
    private readonly IFeedInvoiceCorrectionRepository _feedInvoiceCorrectionRepository;


    public async Task<EmptyBaseResponse> Handle(AddFeedInvoiceCorrectionCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.Data.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), ct);

        var feedInvoices =
            await _feedInvoiceRepository.ListAsync(new GetFeedsInvoicesByIdsSpec(request.Data.FeedInvoiceIds), ct);

        var existedCorrections =
            await _feedInvoiceCorrectionRepository.AnyAsync(
                new GetFeedsCorrectionsByIdsSpec(request.Data.FeedInvoiceIds), ct);

        if (existedCorrections)
        {
            response.AddError("SelectedCorrection", "Zaznaczono korektę podczas dodawania korekty");
            return response;
        }

        if (feedInvoices.Any(t => t.InvoiceCorrectionId.HasValue))
        {
            response.AddError("FeedInvoiceHasCorrection", "Jedna z wybranych faktur dostaw posiada już korektę");
            return response;
        }

        if (feedInvoices.Any(t => t.FarmId != farm.Id))
        {
            response.AddError("FeedInvoiceFarm", "Jedna z wybranych faktur dostaw ma przypisaną inną fermę");
            return response;
        }

        var duplicateInvoiceCheck = await _feedInvoiceCorrectionRepository.AnyAsync(
            new GetFeedCorrectionByInvoiceNumberSpec(request.Data.InvoiceNumber), ct);

        if (duplicateInvoiceCheck)
        {
            response.AddError("InvoiceNumber", "Faktura korygująca o podanym numerze już istnieje");
            return response;
        }

        var comment = string.Format(Comment, request.Data.InvoiceNumber);

        var newCorrection = FeedInvoiceCorrectionEntity.CreateNew(
            farm.Id,
            cycle.Id,
            request.Data.InvoiceNumber,
            request.Data.SubTotal,
            request.Data.VatAmount,
            request.Data.InvoiceTotal,
            request.Data.InvoiceDate,
            userId
        );

        if (request.Data.File != null)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(request.Data.File.FileName);
            var fileName = $"{fileId}{extension}";

            using var memoryStream = new MemoryStream();
            await request.Data.File.CopyToAsync(memoryStream, ct);
            var fileBytes = memoryStream.ToArray();

            var filePath = await _s3Service.UploadFileAsync(fileBytes, FileType.FeedDeliveryCorrection,
                fileName);

            newCorrection.SetFilePath(filePath);
        }

        await _feedInvoiceCorrectionRepository.AddAsync(newCorrection, ct);

        foreach (var feedInvoiceEntity in feedInvoices)
        {
            feedInvoiceEntity.SetAsNullCorrectUnitPrice();
            feedInvoiceEntity.SetInvoiceCorrectionId(newCorrection.Id);
            feedInvoiceEntity.SetComment(comment);
        }

        await _feedInvoiceRepository.UpdateRangeAsync(feedInvoices, ct);

        return response;
    }
}

public sealed class GetFeedCorrectionByInvoiceNumberSpec : BaseSpecification<FeedInvoiceCorrectionEntity>
{
    public GetFeedCorrectionByInvoiceNumberSpec(string invoiceNumber)
    {
        EnsureExists();
        Query.Where(t => t.InvoiceNumber == invoiceNumber);
    }
}

public sealed class GetFeedsCorrectionsByIdsSpec : BaseSpecification<FeedInvoiceCorrectionEntity>
{
    public GetFeedsCorrectionsByIdsSpec(List<Guid> feedInvoiceIds)
    {
        EnsureExists();
        Query.Where(t => feedInvoiceIds.Contains(t.Id));
        Query.OrderBy(t => t.InvoiceNumber);
    }
}

public class AddFeedInvoiceCorrectionCommandValidator : AbstractValidator<AddFeedInvoiceCorrectionCommand>
{
    public AddFeedInvoiceCorrectionCommandValidator()
    {
        RuleFor(t => t.Data.InvoiceNumber)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(t => t.Data.FarmId).NotEmpty();
        RuleFor(t => t.Data.SubTotal);
        RuleFor(t => t.Data.VatAmount);
        RuleFor(t => t.Data.InvoiceTotal);
        RuleFor(t => t.Data.FeedInvoiceIds).NotEmpty();
        RuleFor(t => t.Data.InvoiceDate).NotEmpty().LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}