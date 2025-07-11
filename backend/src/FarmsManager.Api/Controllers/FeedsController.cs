﻿using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Feeds.Names;
using FarmsManager.Application.Commands.Feeds.Prices;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Feeds;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class FeedsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca słowniki filtrów
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetFeedsDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionaries()
    {
        return Ok(await mediator.Send(new GetFeedsDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca nazwy pasz
    /// </summary>
    /// <returns></returns>
    [HttpGet("names")]
    [ProducesResponseType(typeof(BaseResponse<GetFeedsNamesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFeedsNames()
    {
        return Ok(await mediator.Send(new GetFeedsNamesQuery()));
    }

    /// <summary>
    /// Dodaje pole dodatkowe dla sprzedaży
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("names/add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFeedName(AddFeedNameCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa pole dodatkowe dla sprzedaży
    /// </summary>
    /// <param name="feedNameId"></param>
    /// <returns></returns>
    [HttpDelete("names/delete/{feedNameId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSaleFieldExtra([FromRoute] Guid feedNameId)
    {
        return Ok(await mediator.Send(new DeleteFeedNameCommand(feedNameId)));
    }

    /// <summary>
    /// Zwraca ceny pasz
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("prices")]
    [ProducesResponseType(typeof(BaseResponse<GetFeedsPricesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFeedsPrices([FromQuery] GetFeedsPricesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetFeedsPricesQuery(filters)));
    }

    /// <summary>
    /// Dodaje nową cenę paszy
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add-price")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFeedPrice(AddFeedPriceCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktualizuje dane ceny paszy
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update-price/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFeedPrice([FromRoute] Guid id, [FromBody] UpdateFeedPriceCommandDto data)
    {
        return Ok(await mediator.Send(new UpdateFeedPriceCommand(id, data)));
    }

    /// <summary>
    /// Usuwa cene paszy
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete-price/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFeedPrice([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteFeedPriceCommand(id)));
    }
}