using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Dashboard.View)]
public class DashboardController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca słowniki filtrów
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetDashboardDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionaries()
    {
        return Ok(await mediator.Send(new GetDashboardDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca dane dla Dashboardu
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetDashboardDataQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboardData([FromQuery] GetDashboardDataQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetDashboardDataQuery(filters)));
    }
}