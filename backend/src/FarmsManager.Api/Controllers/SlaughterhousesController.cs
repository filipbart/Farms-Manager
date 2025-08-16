using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Slaughterhouses;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Slaughterhouses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Data.SlaughterhousesManage)]
public class SlaughterhousesController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Pobiera wszystkie ubojnie
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetAllSlaughterhousesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllSlaughterhouses()
    {
        return Ok(await mediator.Send(new GetAllSlaughterhousesQuery()));
    }

    /// <summary>
    /// Dodaje nową ubojnię
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddSlaughterhouse(AddSlaughterhouseCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktualizuje ubojnię
    /// </summary>
    /// <param name="slaughterhouseId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update/{slaughterhouseId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateSlaughterhouse([FromRoute] Guid slaughterhouseId,
        UpdateSlaughterhouseDto data)
    {
        return Ok(await mediator.Send(new UpdateSlaughterhouseCommand(slaughterhouseId, data)));
    }

    /// <summary>
    /// Usuwa wybraną ubojnię
    /// </summary>
    /// <param name="slaughterhouseId"></param>
    /// <returns></returns>
    [HttpPost("delete/{slaughterhouseId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSlaughterhouse([FromRoute] Guid slaughterhouseId)
    {
        return Ok(await mediator.Send(new DeleteSlaughterhouseCommand(slaughterhouseId)));
    }
}