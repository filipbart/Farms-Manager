using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Models.AzureDi;
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
    public List<FeedDeliveryInvoiceModel> ExtractedFields { get; init; }
}

public record UploadDeliveriesFilesCommandResponse
{
    public List<UploadDeliveryFileData> Files { get; set; }
}

public record UploadDeliveriesFilesCommand(UploadDeliveriesFilesCommandDto Data) : IRequest<BaseResponse<UploadDeliveriesFilesCommandResponse>>;

public class UploadDeliveriesFilesCommandHandler : IRequestHandler<UploadDeliveriesFilesCommand, BaseResponse<UploadDeliveriesFilesCommandResponse>>
{
    private readonly IS3Service _s3Service;
    private readonly IAzureDiService _azureDiService;
    
    
    public async Task<BaseResponse<UploadDeliveriesFilesCommandResponse>> Handle(UploadDeliveriesFilesCommand request, CancellationToken cancellationToken)
    {
        var response = new UploadDeliveriesFilesCommandResponse();

        foreach (var file in request.Data.Files)
        {
            var fileId = Guid.NewGuid();
            var fileIdString = fileId.ToString();
            
            var preSignedUrl = _s3Service.GeneratePreSignedUrl(fileId.ToString());
            
            
            var fileUrl = await _s3Service.UploadFileAsync(file, fileId.ToString());
            
            
            var feedDeliveryInvoiceModels = await _azureDiService.AnalyzeFeedDeliveryInvoiceAsync(preSignedUrl);
            
            
            
            response.Files.Add(new UploadDeliveryFileData
            {
                DraftId = fileId,
                FileUrl = fileUrl,
                ExtractedFields = feedDeliveryInvoiceModels
            });
        }
    }
}

public class UploadDeliveriesFilesCommandValidator : AbstractValidator<UploadDeliveriesFilesCommand>
{
    public UploadDeliveriesFilesCommandValidator()
    {
        RuleFor(t => t.Data).NotNull().NotEmpty().WithMessage("Lista przesłanych plików jest pusta.");
    }
}