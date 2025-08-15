using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.ProductionData.FlockLosses;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.ProductionData.FlockLosses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/production-data/flock-losses")]
public class FlockLossesController(IMediator mediator) :  BaseController
{
    /// <summary>
    /// Zwraca listę pomiarów upadków i wybrakowań według podanych filtrów
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetProductionDataFlockLossesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFlockLosses([FromQuery] GetProductionDataFlockLossesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetProductionDataFlockLossesQuery(filters)));
    }

    /// <summary>
    /// Dodaje nowy pomiar upadków i wybrakowań
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFlockLoss(AddProductionDataFlockLossCommand command)
    {
        return Ok(await mediator.Send(command));
    }


    /// <summary>
    /// Aktualizuje dane pomiaru
    /// </summary>
    [HttpPatch("update/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFlockLoss([FromRoute] Guid id,
        [FromBody] UpdateProductionDataFlockLossCommandDto payload)
    {
        return Ok(await mediator.Send(new UpdateProductionDataFlockLossCommand(id, payload)));
    }

    /// <summary>
    /// Usuwa pomiar
    /// </summary>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFlockLoss([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteProductionDataFlockLossCommand(id)));
    }
}