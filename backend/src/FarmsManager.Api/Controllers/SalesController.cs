﻿using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Sales;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Queries.Sales;
using FarmsManager.Application.Queries.Sales.Dictionary;
using FarmsManager.Application.Queries.Sales.ExportFile;
using FarmsManager.Shared.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class SalesController(IMediator mediator, IS3Service s3Service) : BaseController
{
    /// <summary>
    /// Zwraca słowniki filtrów
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetSalesDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionaries()
    {
        return Ok(await mediator.Send(new GetSalesDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca sprzedaże według podanych filtrów
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetSalesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSales([FromQuery] GetSalesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetSalesQuery(filters)));
    }

    /// <summary>
    /// Dodaje nową sprzedaz
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(BaseResponse<AddNewSaleCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNewInsertion([FromForm] AddNewSaleCommandDto dto)
    {
        var entries = dto.Entries.ParseJsonString<List<AddNewSaleCommand.Entry>>();

        var command = new AddNewSaleCommand
        {
            SaleType = dto.SaleType,
            FarmId = dto.FarmId,
            CycleId = dto.CycleId,
            SaleDate = dto.SaleDate,
            SlaughterhouseId = dto.SlaughterhouseId,
            Entries = entries ?? [],
            Files = dto.Files
        };

        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktualizuje dane sprzedaży
    /// </summary>
    /// <param name="id"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPatch("update/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateInsertin([FromRoute] Guid id, [FromBody] UpdateSaleCommandDto payload)
    {
        return Ok(await mediator.Send(new UpdateSaleCommand(id, payload)));
    }

    /// <summary>
    /// Usuwa sprzedaż
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteSaleCommand(id)));
    }

    /// <summary>
    /// Wysyła sprzedaż do systemu IRZplus
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("send-to-irz")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendToIrzPlus(SendSaleToIrzCommand command)
    {
        return Ok(await mediator.Send(command));
    }


    [HttpGet("export")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSaleExportFile([FromQuery] GetSalesQueryFilters filters)
    {
        const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileStream = await mediator.Send(new GetSalesExportFileQuery(filters));

        return fileStream is null ? NoContent() : File(fileStream, contentType);
    }

    /// <summary>
    /// Zwraca folder sprzedaży
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [HttpGet("download")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSaleDownloadFile([FromQuery] string path)
    {
        var decodedPath = Uri.UnescapeDataString(path);
        var file = await s3Service.GetFolderAsZipAsync(decodedPath);


        return file is null ? NotFound() : File(file.Data, file.ContentType, file.FileName);
    }
}