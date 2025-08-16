using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.ProductionData.RemainingFeed;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ProductionData;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.ProductionData.RemainingFeed;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/production-data/remaining-feed")]
[HasPermission(AppPermissions.ProductionData.RemainingFeedView)]
public class RemainingFeedController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca listę pozostałej paszy według podanych filtrów
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetProductionDataRemainingFeedQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get([FromQuery] ProductionDataQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetProductionDataRemainingFeedQuery(filters)));
    }

    /// <summary>
    /// Dodaje nowy wpis o pozostałej paszy
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddRemainingFeed(AddProductionDataRemainingFeedCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktualizuje dane wpisu
    /// </summary>
    [HttpPatch("update/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFailure([FromRoute] Guid id,
        [FromBody] UpdateProductionDataRemainingFeedCommandDto payload)
    {
        return Ok(await mediator.Send(new UpdateProductionDataRemainingFeedCommand(id, payload)));
    }

    /// <summary>
    /// Usuwa wpis
    /// </summary>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFailure([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteProductionDataRemainingFeedCommand(id)));
    }
}