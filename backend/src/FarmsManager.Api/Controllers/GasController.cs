using FarmsManager.Api.Controllers.Base;
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
}