using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Files;

public record GetFilesByFileTypeQuery(FileType FileType) : IRequest<BaseResponse<PaginationModel<FileModel>>>;

public class
    GetFilesByFileTypeQueryHandler : IRequestHandler<GetFilesByFileTypeQuery, BaseResponse<PaginationModel<FileModel>>>
{
    private readonly IS3Service _s3Service;

    public GetFilesByFileTypeQueryHandler(IS3Service s3Service)
    {
        _s3Service = s3Service;
    }

    public async Task<BaseResponse<PaginationModel<FileModel>>> Handle(GetFilesByFileTypeQuery request,
        CancellationToken cancellationToken)
    {
        var files = await _s3Service.GetFilesByType(request.FileType);
        return new BaseResponse<PaginationModel<FileModel>>(files);
    }
}