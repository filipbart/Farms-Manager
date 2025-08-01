using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Hatcheries;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Hatcheries;
using FarmsManager.Application.Queries.Hatcheries.Prices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class HatcheriesController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Pobiera wszystkie wylęgarnie
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetAllHatcheriesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllHatcheries()
    {
        return Ok(await mediator.Send(new GetAllHatcheriesQuery()));
    }

    /// <summary>
    /// Dodaje nową wylęgarnię
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddHatchery(AddHatcheryCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa wybraną wylęgarnię
    /// </summary>
    /// <param name="hatcheryId"></param>
    /// <returns></returns>
    [HttpPost("delete/{hatcheryId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteHatchery([FromRoute] Guid hatcheryId)
    {
        return Ok(await mediator.Send(new DeleteHatcheryCommand(hatcheryId)));
    }

    /// <summary>
    /// Zwraca słownik dla cen
    /// </summary>
    /// <returns></returns>
    [HttpGet("prices/dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetHatcheryPricesDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPricesDictionary()
    {
        return Ok(await mediator.Send(new GetHatcheryPricesDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca ceny 
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("prices")]
    [ProducesResponseType(typeof(BaseResponse<GetHatcheryPricesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHatcheryPrices([FromQuery] GetHatcheryPricesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetHatcheryPricesQuery(filters)));
    }

    /// <summary>
    /// Dodaje cenę
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("prices/add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddHatcheryPrice(AddHatcheryPriceCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktulizuję cenę
    /// </summary>
    /// <param name="hatcheryPriceId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("prices/update/{hatcheryPriceId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateHatcheryPrice([FromRoute] Guid hatcheryPriceId, UpdateHatcheryPriceData data)
    {
        return Ok(await mediator.Send(new UpdateHatcheryPriceCommand(hatcheryPriceId, data)));
    }

    /// <summary>
    /// Usuwa cenę
    /// </summary>
    /// <param name="hatcheryPriceId"></param>
    /// <returns></returns>
    [HttpDelete("prices/delete/{hatcheryPriceId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteHatcheryPrice([FromRoute] Guid hatcheryPriceId)
    {
        return Ok(await mediator.Send(new DeleteHatcheryPriceCommand(hatcheryPriceId)));
    }
}