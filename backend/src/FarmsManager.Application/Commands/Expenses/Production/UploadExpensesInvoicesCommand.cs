using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.AzureDi;
using FarmsManager.Application.Models.Invoices;
using FarmsManager.Application.Specifications.Expenses;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Entities;
using FarmsManager.Domain.Aggregates.ExpenseAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Expenses.Production;

public record UploadExpensesInvoicesDto
{
    public List<IFormFile> Files { get; set; } = [];
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
    private readonly IFarmRepository _farmRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IExpenseContractorRepository _expenseContractorRepository;
    private readonly IExpenseProductionRepository _expenseProductionRepository;


    public UploadExpensesInvoicesCommandHandler(IMapper mapper, IS3Service s3Service, IAzureDiService azureDiService,
        IFarmRepository farmRepository, IUserDataResolver userDataResolver,
        IExpenseContractorRepository expenseContractorRepository,
        IExpenseProductionRepository expenseProductionRepository)
    {
        _mapper = mapper;
        _s3Service = s3Service;
        _azureDiService = azureDiService;
        _farmRepository = farmRepository;
        _userDataResolver = userDataResolver;
        _expenseContractorRepository = expenseContractorRepository;
        _expenseProductionRepository = expenseProductionRepository;
    }

    public async Task<BaseResponse<UploadExpensesInvoicesCommandResponse>> Handle(UploadExpensesInvoicesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var response = BaseResponse.CreateResponse(new UploadExpensesInvoicesCommandResponse());

        foreach (var file in request.Dto.Files)
        {
            var fileId = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var newFileName = fileId + extension;
            var filePath = "draft/" + fileId + extension;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();
            var key = await _s3Service.UploadFileAsync(fileBytes, FileType.ExpenseProduction, filePath,
                cancellationToken: cancellationToken);

            var preSignedUrl =
                _s3Service.GeneratePreSignedUrl(FileType.ExpenseProduction, filePath, newFileName);

            var expenseProductionInvoiceModel =
                await _azureDiService.AnalyzeInvoiceAsync<ExpenseProductionInvoiceModel>(preSignedUrl,
                    cancellationToken);
            var extractedFields = _mapper.Map<AddExpenseProductionInvoiceDto>(expenseProductionInvoiceModel);

            if (expenseProductionInvoiceModel is null)
            {
                throw new Exception("Faktura nie jest możliwa do odczytu");
            }

            // Szukaj fermy najpierw po NIP, potem po nazwie
            FarmEntity farm = null;

            if (!string.IsNullOrWhiteSpace(expenseProductionInvoiceModel.CustomerNip))
            {
                farm = await _farmRepository.FirstOrDefaultAsync(
                    new FarmByNipSpec(expenseProductionInvoiceModel.CustomerNip?.Replace("-", "")),
                    cancellationToken);
            }

            if (farm is null && !string.IsNullOrWhiteSpace(expenseProductionInvoiceModel.CustomerName))
            {
                farm = await _farmRepository.FirstOrDefaultAsync(
                    new FarmByNameSpec(expenseProductionInvoiceModel.CustomerName),
                    cancellationToken);
            }

            var expenseContractor = await _expenseContractorRepository.FirstOrDefaultAsync(
                new ExpenseContractorByNipSpec(expenseProductionInvoiceModel.VendorNip?.Replace("-", "")),
                cancellationToken);

            if (expenseContractor is null && expenseProductionInvoiceModel.VendorNip.IsNotEmpty())
            {
                expenseContractor = ExpenseContractorEntity.CreateNewFromInvoice(
                    expenseProductionInvoiceModel.VendorName, expenseProductionInvoiceModel.VendorNip ?? string.Empty,
                    expenseProductionInvoiceModel.VendorAddress.Replace("\n", ""), userId);
                await _expenseContractorRepository.AddAsync(expenseContractor, cancellationToken);
            }

            // Sprawdź czy dla danego sprzedawcy nie istnieje już faktura z tym numerem
            if (expenseContractor is not null)
            {
                var existedInvoice = await _expenseProductionRepository.FirstOrDefaultAsync(
                    new GetExpenseProductionInvoiceByInvoiceNumberAndContractorSpec(extractedFields.InvoiceNumber,
                        expenseContractor.Id),
                    cancellationToken);

                if (existedInvoice is not null)
                {
                    throw DomainException.BadRequest(
                        $"Istnieje już koszt produkcyjny z numerem faktury '{existedInvoice.InvoiceNumber}' dla sprzedawcy '{expenseContractor.Name}'.");
                }
            }

            extractedFields.FarmId = farm?.Id;
            extractedFields.CycleId = farm?.ActiveCycleId;
            extractedFields.ContractorId = expenseContractor?.Id;
            extractedFields.ExpenseTypeId = expenseContractor?.ExpenseTypeId;
            extractedFields.ExpenseTypeName = expenseContractor?.ExpenseType?.Name;

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
            .ForMember(m => m.ContractorId, opt => opt.Ignore())
            .ForMember(m => m.ExpenseTypeName, opt => opt.Ignore())
            .ForMember(m => m.ExpenseTypeId, opt => opt.Ignore());
    }
}