using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.ProductionData.TransferFeed;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ProductionData;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.ProductionData.TransferFeed;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/production-data/transfer-feed")]
[HasPermission(AppPermissions.ProductionData.TransferFeedView)]
public class TransferFeedController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca listę transferu paszy według podanych filtrów
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetProductionDataTransferFeedQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get([FromQuery] ProductionDataQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetProductionDataTransferFeedQuery(filters)));
    }
    
    /// <summary>
    /// Dodaje nowy wpis o przeniesieniu paszy
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddRemainingFeed(AddProductionDataTransferFeedCommand command)
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
        [FromBody] UpdateProductionDataTransferFeedCommandDto payload)
    {
        return Ok(await mediator.Send(new UpdateProductionDataTransferFeedCommand(id, payload)));
    }

    /// <summary>
    /// Usuwa wpis
    /// </summary>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFailure([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteProductionDataTransferFeedCommand(id)));
    }
}