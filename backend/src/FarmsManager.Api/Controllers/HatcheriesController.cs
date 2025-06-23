using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Hatcheries;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Hatcheries;
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
}