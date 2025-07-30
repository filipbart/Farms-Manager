using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ProductionData;
using FarmsManager.Application.Queries.ProductionData.Weighings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/production-data/weighings")]
public class WeighingsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca listę wpisów o upadkach i wybrakowaniach według podanych filtrów
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetProductionDataWeighingsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetWeighings([FromQuery] ProductionDataQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetProductionDataWeighingsQuery(filters)));
    }

    // /// <summary>
    // /// Dodaje nowy wpis o upadkach/wybrakowaniach
    // /// </summary>
    // [HttpPost("add")]
    // [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // public async Task<IActionResult> AddFailure(AddProductionDataFailureCommand command)
    // {
    //     return Ok(await mediator.Send(command));
    // }
    //
    // /// <summary>
    // /// Aktualizuje dane wpisu
    // /// </summary>
    // [HttpPatch("update/{id:guid}")]
    // [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // public async Task<IActionResult> UpdateFailure([FromRoute] Guid id,
    //     [FromBody] UpdateProductionDataFailureCommandDto payload)
    // {
    //     return Ok(await mediator.Send(new UpdateProductionDataFailureCommand(id, payload)));
    // }
    //
    // /// <summary>
    // /// Usuwa wpis
    // /// </summary>
    // [HttpDelete("delete/{id:guid}")]
    // [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // public async Task<IActionResult> DeleteFailure([FromRoute] Guid id)
    // {
    //     return Ok(await mediator.Send(new DeleteProductionDataFailureCommand(id)));
    // }
}