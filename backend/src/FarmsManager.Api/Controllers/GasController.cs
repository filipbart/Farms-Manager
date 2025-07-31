using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Gas;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Gas;
using FarmsManager.Application.Queries.Gas.Deliveries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class GasController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca słownik filtrów dla dostaw gazu
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetGasDeliveriesDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGasDeliveriesDictionary()
    {
        return Ok(await mediator.Send(new GetGasDeliveriesDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca dostawy gazu
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("deliveries")]
    [ProducesResponseType(typeof(BaseResponse<GetGasDeliveriesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGasDeliveries([FromQuery] GetGasDeliveriesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetGasDeliveriesQuery(filters)));
    }

    /// <summary>
    /// Zwraca kontrahentów dostaw gazu
    /// </summary>
    /// <returns></returns>
    [HttpGet("contractors")]
    [ProducesResponseType(typeof(BaseResponse<GetGasContractorsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGasContractors()
    {
        return Ok(await mediator.Send(new GetGasContractorsQuery()));
    }

    /// <summary>
    /// Wrzuca tymczasowo pliki faktur i je odczytuje dla dostawy gazu
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("deliveries/upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(BaseResponse<UploadGasDeliveriesInvoicesCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadExpenseInvoices([FromForm] UploadGasDeliveriesInvoicesDto dto)
    {
        return Ok(await mediator.Send(new UploadGasDeliveriesInvoicesCommand(dto)));
    }

    /// <summary>
    /// Dodaje nową dostawę gazu
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("deliveries/add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddGasDelivery([FromForm] AddGasDeliveryData data)
    {
        return Ok(await mediator.Send(new AddGasDeliveryCommand(data)));
    }
}