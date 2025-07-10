using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Sales;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Sales;
using FarmsManager.Application.Queries.Sales.Dictionary;
using FarmsManager.Application.Queries.Sales.ExportFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class SalesController(IMediator mediator) : BaseController
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
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(BaseResponse<AddNewSaleCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNewInsertion(AddNewSaleCommand command)
    {
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
}