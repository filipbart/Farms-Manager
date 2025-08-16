using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.ProductionData.Failures;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ProductionData;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.ProductionData.Failures;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/production-data/failures")]
[HasPermission(AppPermissions.ProductionData.FailuresView)]
public class FailuresController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca listę wpisów o upadkach i wybrakowaniach według podanych filtrów
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetProductionDataFailuresQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFailures([FromQuery] ProductionDataQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetProductionDataFailuresQuery(filters)));
    }

    /// <summary>
    /// Dodaje nowy wpis o upadkach/wybrakowaniach
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFailure(AddProductionDataFailureCommand command)
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
        [FromBody] UpdateProductionDataFailureCommandDto payload)
    {
        return Ok(await mediator.Send(new UpdateProductionDataFailureCommand(id, payload)));
    }

    /// <summary>
    /// Usuwa wpis
    /// </summary>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFailure([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteProductionDataFailureCommand(id)));
    }
}