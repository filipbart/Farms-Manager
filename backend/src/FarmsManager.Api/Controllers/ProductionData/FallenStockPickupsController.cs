using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.FallenStockPickups;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.FallenStockPickups;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/fallen-stock-pickups")]
[HasPermission(AppPermissions.ProductionData.IrzPlusView)]
public class FallenStockPickupsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca sztuki padłe odebrane przez zakład utylizacyjny
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetFallenStockPickupsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFallenStockPickups([FromQuery] GetFallenStockPickupsQuery query)
    {
        return Ok(await mediator.Send(query));
    }

    /// <summary>
    /// Dodaje nowe odbioru sztuk padłych
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNewFallenStockPickups(AddNewFallenStockPickupsCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa sztuki padłe odebrane
    /// </summary>
    /// <param name="fallenStockPickupId"></param>
    /// <returns></returns>
    [HttpDelete("delete/{fallenStockPickupId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFallenStockPickup([FromRoute] Guid fallenStockPickupId)
    {
        return Ok(await mediator.Send(new DeleteFallenStockPickupCommand(fallenStockPickupId)));
    }

    /// <summary>
    /// Aktualizuje dane sztuk padłych odebranych
    /// </summary>
    /// <param name="fallenStockPickupId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update/{fallenStockPickupId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFallenStockPickup([FromRoute] Guid fallenStockPickupId,
        [FromBody] UpdateFallenStockPickupDto data)
    {
        return Ok(await mediator.Send(new UpdateFallenStockPickupCommand(fallenStockPickupId, data)));
    }
}