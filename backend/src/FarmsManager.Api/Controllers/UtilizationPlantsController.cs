using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.UtilizationPlants;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.UtilizationPlants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[Route("api/utilization-plants")]
public class UtilizationPlantsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Pobiera wszystkie zakłady utylizacyjne
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetAllUtilizationPlantsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllUtilizationPlants()
    {
        return Ok(await mediator.Send(new GetAllUtilizationPlantsQuery()));
    }

    /// <summary>
    /// Dodaje nowy zakład utylizacyjny
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddUtilizationPlant(AddUtilizationPlantCommand command)
    {
        return Ok(await mediator.Send(command));
    }
    
    /// <summary>
    /// Aktualizuje zakład utylizacyjny
    /// </summary>
    /// <param name="utilizationPlantId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update/{utilizationPlantId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUtilizationPlant([FromRoute] Guid utilizationPlantId,
        UpdateUtilizationPlantDto data)
    {
        return Ok(await mediator.Send(new UpdateUtilizationPlantCommand(utilizationPlantId, data)));
    }

    /// <summary>
    /// Usuwa wybrany zakład utylizacyjny
    /// </summary>
    /// <param name="utilizationPlantId"></param>
    /// <returns></returns>
    [HttpDelete("delete/{utilizationPlantId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteUtilizationPlant([FromRoute] Guid utilizationPlantId)
    {
        return Ok(await mediator.Send(new DeleteUtilizationPlantCommand(utilizationPlantId)));
    }
}