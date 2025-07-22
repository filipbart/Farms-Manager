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
    /// <param name="path"></param>
    /// <param name="fileType"></param>
    /// <returns></returns>
    [HttpGet("{path}")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFile([FromRoute] string path, [FromQuery] FileType fileType)
    {
        var file = await s3Service.GetFileAsync(fileType, path);
        return file is null ? NotFound() : File(file.Data, file.ContentType, file.FileName);
    }
}