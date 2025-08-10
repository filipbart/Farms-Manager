using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.ColumnsViews;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.ColumnsViews;
using FarmsManager.Domain.Aggregates.SeedWork.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[Route("api/columns-views")]
public class ColumnsViewsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca widoku kolumn dla określonego typu
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetColumnsViewsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetColumnsViews([FromQuery] ColumnViewType type)
    {
        return Ok(await mediator.Send(new GetColumnsViewsQuery(type)));
    }

    /// <summary>
    /// Dodaje widok kolumn
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddColumnView(AddColumnViewCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa widok kolumn
    /// </summary>
    /// <param name="columnViewId"></param>
    /// <returns></returns>
    [HttpDelete("delete/{columnViewId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteColumnView([FromRoute] Guid columnViewId)
    {
        return Ok(await mediator.Send(new DeleteColumnViewCommand(columnViewId)));
    }
}