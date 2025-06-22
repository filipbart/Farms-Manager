using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Farms;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class FarmsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Pobiera całą listę ferm
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetAllFarmsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllFarms()
    {
        return Ok(await mediator.Send(new GetAllFarmsQuery()));
    }

    /// <summary>
    /// Pobiera całą listę kurników fermy
    /// </summary>
    /// <param name="farmId"></param>
    /// <returns></returns>
    [HttpGet("{farmId:guid}/henhouses")]
    [ProducesResponseType(typeof(BaseResponse<GetFarmHenhousesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFarmHenhouses([FromRoute] Guid farmId)
    {
        return Ok(await mediator.Send(new GetFarmHenhousesQuery(farmId)));
    }

    /// <summary>
    /// Dodaje nową fęrmę
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFarm(AddFarmCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa wybraną fermę
    /// </summary>
    /// <param name="farmId"></param>
    /// <returns></returns>
    [HttpPost("delete/{farmId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFarm([FromRoute] Guid farmId)
    {
        return Ok(await mediator.Send(new DeleteFarmCommand(farmId)));
    }

    /// <summary>
    /// Dodaje kurnik do fermy
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("{farmId:guid}/add-henhouse")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddHenhouse(AddHenhouseCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa wybraną fermę
    /// </summary>
    /// <param name="henhouseId"></param>
    /// <returns></returns>
    [HttpPost("henhouse/delete/{henhouseId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteHenhouse([FromRoute] Guid henhouseId)
    {
        return Ok(await mediator.Send(new DeleteHenhouseCommand(henhouseId)));
    }
}