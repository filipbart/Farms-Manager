using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.ProductionData.Weighings;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.ProductionData.Weighings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/production-data/weighings/standards")]
[HasPermission(AppPermissions.ProductionData.WeighingsView)]
public class WeightStandardsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca listę wszystkich norm wagowych
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetWeightStandardsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetStandards()
    {
        return Ok(await mediator.Send(new GetWeightStandardsQuery()));
    }

    /// <summary>
    /// Dodaje nowe normy wagowe
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddStandards(AddWeightStandardsCommand command)
    {
        return Ok(await mediator.Send(command));
    }
    
    /// <summary>
    /// Usuwa normę wagową
    /// </summary>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteStandard([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteWeightStandardCommand(id)));
    }
}