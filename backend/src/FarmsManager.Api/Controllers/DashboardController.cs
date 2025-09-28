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
    /// Zwraca statystyki i statusy kurników
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(BaseResponse<GetDashboardStatsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetStats([FromQuery] GetDashboardDataQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetDashboardStatsQuery(filters)));
    }

    /// <summary>
    /// Zwraca powiadomienia
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("notifications")]
    [ProducesResponseType(typeof(BaseResponse<List<DashboardNotificationItem>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotifications([FromQuery] GetDashboardDataQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetDashboardNotificationsQuery(filters)));
    }

    /// <summary>
    /// Zwraca dane do wszystkich wykresów liniowych (FCR, EWW, Zużycie gazu, Straty)
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("charts")]
    [ProducesResponseType(typeof(BaseResponse<GetDashboardChartsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCharts([FromQuery] GetDashboardDataQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetDashboardChartsQuery(filters)));
    }
    
    /// <summary>
    /// Zwraca dane do wykresu kołowego wydatków
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("expenses-pie-chart")]
    [ProducesResponseType(typeof(BaseResponse<DashboardExpensesPieChart>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpensesPieChart([FromQuery] GetDashboardDataQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetExpensesPieChartQuery(filters)));
    }
}