using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.AzureDi;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Accounting;

public record UploadAccountingInvoicesCommandDto
{
    public List<IFormFile> Files { get; init; }
    public string InvoiceType { get; init; } // Purchase or Sales
}

/// <summary>
/// Dane zaczytane z faktury przez AI
/// </summary>
public record AccountingInvoiceExtractedData
{
    public string InvoiceNumber { get; init; }
    public string InvoiceDate { get; init; }
    public string DueDate { get; init; }
    public string SellerName { get; init; }
    public string SellerNip { get; init; }
    public string SellerAddress { get; init; }
    public string BuyerName { get; init; }
    public string BuyerNip { get; init; }
    public string BuyerAddress { get; init; }
    public decimal? GrossAmount { get; init; }
    public decimal? NetAmount { get; init; }
    public decimal? VatAmount { get; init; }
    public string BankAccountNumber { get; init; }
    public string InvoiceType { get; init; }
}

public record UploadAccountingInvoiceFileData
{
    public Guid DraftId { get; init; }
    public string FileUrl { get; init; }
    public string FilePath { get; init; }
    public AccountingInvoiceExtractedData ExtractedFields { get; init; }
}

public record UploadAccountingInvoicesCommandResponse
{
    public List<UploadAccountingInvoiceFileData> Files { get; set; } = [];
}

public record UploadAccountingInvoicesCommand(UploadAccountingInvoicesCommandDto Data)
    : IRequest<BaseResponse<UploadAccountingInvoicesCommandResponse>>;

public class UploadAccountingInvoicesCommandHandler : IRequestHandler<UploadAccountingInvoicesCommand,
    BaseResponse<UploadAccountingInvoicesCommandResponse>>
{
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;
    private readonly IAzureDiService _azureDiService;
    private readonly IUserDataResolver _userDataResolver;

    public UploadAccountingInvoicesCommandHandler(
        IMapper mapper,
        IS3Service s3Service,
        IAzureDiService azureDiService,
        IUserDataResolver userDataResolver)
    {
        _mapper = mapper;
        _s3Service = s3Service;
        _azureDiService = azureDiService;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<UploadAccountingInvoicesCommandResponse>> Handle(
        UploadAccountingInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = new UploadAccountingInvoicesCommandResponse();

        foreach (var file in request.Data.Files)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var newFileName = fileId + extension;
            var filePath = "accounting/draft/" + fileId + extension;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.AccountingInvoice, filePath,
                cancellationToken);

            var preSignedUrl = _s3Service.GeneratePreSignedUrl(FileType.AccountingInvoice, filePath, newFileName);

            // Zaczytaj dane z faktury przez AI
            var invoiceModel =
                await _azureDiService.AnalyzeInvoiceAsync<AccountingInvoiceModel>(preSignedUrl, cancellationToken);

            var extractedFields = _mapper.Map<AccountingInvoiceExtractedData>(invoiceModel);
            extractedFields = extractedFields with { InvoiceType = request.Data.InvoiceType };

            response.Files.Add(new UploadAccountingInvoiceFileData
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

public class UploadAccountingInvoicesCommandValidator : AbstractValidator<UploadAccountingInvoicesCommand>
{
    public UploadAccountingInvoicesCommandValidator()
    {
        RuleFor(t => t.Data).NotNull().WithMessage("Dane są wymagane.");
        RuleFor(t => t.Data.Files).NotNull().NotEmpty().WithMessage("Lista przesłanych plików jest pusta.");
        RuleFor(t => t.Data.InvoiceType).NotEmpty().WithMessage("Typ faktury jest wymagany.");
    }
}

public class UploadAccountingInvoicesCommandProfile : Profile
{
    public UploadAccountingInvoicesCommandProfile()
    {
        CreateMap<AccountingInvoiceModel, AccountingInvoiceExtractedData>()
            .ForMember(m => m.InvoiceDate,
                opt => opt.MapFrom(t => t.InvoiceDate.HasValue ? t.InvoiceDate.Value.ToString("yyyy-MM-dd") : null))
            .ForMember(m => m.DueDate,
                opt => opt.MapFrom(t => t.DueDate.HasValue ? t.DueDate.Value.ToString("yyyy-MM-dd") : null))
            .ForMember(m => m.InvoiceType, opt => opt.Ignore());
    }
}
