using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models;
using FarmsManager.Application.Models.AzureDi;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Expenses.Production;

public record UploadExpensesInvoicesDto
{
    public List<IFormFile> Files { get; init; }
}

public record UploadExpensesInvoicesData
{
    public Guid DraftId { get; init; }
    public string FileUrl { get; init; }
    public string FilePath { get; init; }
    public AddExpenseProductionInvoiceDto ExtractedFields { get; init; }
}

public record UploadExpensesInvoicesCommandResponse
{
    public List<UploadExpensesInvoicesData> Files { get; set; } = [];
}

public record UploadExpensesInvoicesCommand(UploadExpensesInvoicesDto Dto)
    : IRequest<BaseResponse<UploadExpensesInvoicesCommandResponse>>;

public class UploadExpensesInvoicesCommandHandler : IRequestHandler<UploadExpensesInvoicesCommand,
    BaseResponse<UploadExpensesInvoicesCommandResponse>>
{
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;
    private readonly IAzureDiService _azureDiService;
    private readonly IExpenseProductionRepository _expenseProductionRepository;

    public UploadExpensesInvoicesCommandHandler(IMapper mapper, IS3Service s3Service, IAzureDiService azureDiService,
        IExpenseProductionRepository expenseProductionRepository)
    {
        _mapper = mapper;
        _s3Service = s3Service;
        _azureDiService = azureDiService;
        _expenseProductionRepository = expenseProductionRepository;
    }

    public async Task<BaseResponse<UploadExpensesInvoicesCommandResponse>> Handle(UploadExpensesInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        var response = BaseResponse.CreateResponse(new UploadExpensesInvoicesCommandResponse());

        foreach (var file in request.Dto.Files)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var filePath = "draft/" + fileId + extension;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.ExpenseProduction, filePath);

            var preSignedUrl = _s3Service.GeneratePreSignedUrl(FileType.ExpenseProduction, filePath, file.FileName);

            var expenseProductionInvoiceModel = await _azureDiService.AnalyzeFeedDeliveryInvoiceAsync(preSignedUrl);
            var extractedFields = _mapper.Map<AddExpenseProductionInvoiceDto>(expenseProductionInvoiceModel);

            var existedInvoice = await _expenseProductionRepository.FirstOrDefaultAsync(
                new GetExpenseProductionInvoiceByInvoiceNumberSpec(extractedFields.InvoiceNumber), cancellationToken);

            if (existedInvoice is not null)
            {
                throw new Exception($"Istnieje już dostawa z tym numerem faktury: {existedInvoice.InvoiceNumber}");
            }

            //todo wyciągnąć dane z invoiceModel i szukać Id fermy itd.

            response.ResponseData.Files.Add(new UploadExpensesInvoicesData
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

public class UploadExpensesInvoicesCommandValidator : AbstractValidator<UploadExpensesInvoicesCommand>
{
    public UploadExpensesInvoicesCommandValidator()
    {
        RuleFor(t => t.Dto).NotEmpty().WithMessage("Lista przesłanych plików jest pusta.");
    }
}

public class UploadExpensesInvoicesCommandProfile : Profile
{
    public UploadExpensesInvoicesCommandProfile()
    {
        CreateMap<ExpenseProductionInvoiceModel, AddExpenseProductionInvoiceDto>()
            .ForMember(m => m.FarmId, opt => opt.Ignore())
            .ForMember(m => m.CycleId, opt => opt.Ignore())
            .ForMember(m => m.ContractorName, opt => opt.Ignore())
            .ForMember(m => m.ExpenseTypeName, opt => opt.Ignore())
            .ForMember(m => m.ExpenseTypeId, opt => opt.Ignore());
    }
}