using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Common;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Files;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class FilesController(IMediator mediator, IS3Service s3Service) : BaseController
{
    /// <summary>
    /// Zwraca pliki w wybranym folderze
    /// </summary>
    /// <param name="fileType"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PaginationModel<FileModel>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFilesByType([FromQuery] FileType fileType)
    {
        return Ok(await mediator.Send(new GetFilesByFileTypeQuery(fileType)));
    }

    /// <summary>
    /// Zwraca plik
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileType"></param>
    /// <returns></returns>
    [HttpGet("file")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFile([FromQuery] string filePath, [FromQuery] FileType? fileType)
    {
        var decodedPath = Uri.UnescapeDataString(filePath);
        var file = fileType.HasValue == false
            ? await s3Service.GetFileByKeyAsync(decodedPath)
            : await s3Service.GetFileAsync(fileType.Value, decodedPath);

        return file is null ? NotFound() : File(file.Data, file.ContentType, file.FileName);
    }
}