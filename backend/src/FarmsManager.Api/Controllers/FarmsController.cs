using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Farms;
using FarmsManager.Domain.Models.FarmAggregate;
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
    /// Pobiera aktualny cykl dla farmy
    /// </summary>
    /// <param name="farmId"></param>
    /// <returns></returns>
    [HttpGet("{farmId:guid}/latest-cycle")]
    [ProducesResponseType(typeof(BaseResponse<CycleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLatestCycle([FromRoute] Guid farmId)
    {
        return Ok(await mediator.Send(new GetFarmLatestCycleQuery(farmId)));
    }

    /// <summary>
    /// Zwraca cykle fermy
    /// </summary>
    /// <param name="farmId"></param>
    /// <returns></returns>
    [HttpGet("{farmId:guid}/cycles")]
    [ProducesResponseType(typeof(BaseResponse<List<CycleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFarmCycles([FromRoute] Guid farmId)
    {
        return Ok(await mediator.Send(new GetFarmCyclesQuery(farmId)));
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
    /// Dodaje nową fermę
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [HasPermission(AppPermissions.Data.FarmsManage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFarm(AddFarmCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktualizuje dane fermy
    /// </summary>
    /// <param name="farmId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update/{farmId:guid}")]
    [HasPermission(AppPermissions.Data.FarmsManage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFarm([FromRoute] Guid farmId, UpdateFarmDto data)
    {
        return Ok(await mediator.Send(new UpdateFarmCommand(farmId, data)));
    }

    /// <summary>
    /// Usuwa wybraną fermę
    /// </summary>
    /// <param name="farmId"></param>
    /// <returns></returns>
    [HttpPost("delete/{farmId:guid}")]
    [HasPermission(AppPermissions.Data.FarmsManage)]
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
    [HasPermission(AppPermissions.Data.HousesManage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddHenhouse(AddHenhouseCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa wybrany kurnik
    /// </summary>
    /// <param name="henhouseId"></param>
    /// <returns></returns>
    [HttpPost("henhouse/delete/{henhouseId:guid}")]
    [HasPermission(AppPermissions.Data.HousesManage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteHenhouse([FromRoute] Guid henhouseId)
    {
        return Ok(await mediator.Send(new DeleteHenhouseCommand(henhouseId)));
    }

    /// <summary>
    /// Aktualizuje wybrany kurnik
    /// </summary>
    /// <param name="henhouseId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("henhouse/update/{henhouseId:guid}")]
    [HasPermission(AppPermissions.Data.HousesManage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateHenhouse([FromRoute] Guid henhouseId, UpdateHenhouseDto data)
    {
        return Ok(await mediator.Send(new UpdateHenhouseCommand(henhouseId, data)));
    }

    /// <summary>
    /// Aktualizuje cykl dla danej fermy
    /// </summary>
    /// <returns></returns>
    [HttpPost("update-cycle")]
    [HasPermission(AppPermissions.Settings.Cycles.Manage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFarmCycle(UpdateFarmCycleCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Pobiera listę kurników z wstawieniami dla danej fermy i cyklu
    /// </summary>
    /// <param name="query">Identyfikatory</param>
    /// <returns>Lista kurników z wstawieniami</returns>
    [HttpGet("inserted-henhouses")]
    [ProducesResponseType(typeof(BaseResponse<List<HenhouseRowDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInsertedHenhouses([FromQuery] GetInsertedHenhousesQuery query)
    {
        return Ok(await mediator.Send(query));
    }
}