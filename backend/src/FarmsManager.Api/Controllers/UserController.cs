using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class UserController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca podstawowe dane o użytkowniku
    /// </summary>
    /// <returns></returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(BaseResponse<MeQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me()
    {
        return Ok(await mediator.Send(new MeQuery()));
    }

    /// <summary>
    /// Zwraca szczegóły użytkownika
    /// </summary>
    /// <returns></returns>
    [HttpGet("details")]
    [ProducesResponseType(typeof(BaseResponse<MeQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Details()
    {
        return Ok(await mediator.Send(new DetailsQuery()));
    }

    /// <summary>
    /// Zwraca alerty
    /// </summary>
    /// <returns></returns>
    [HttpGet("notifications")]
    [ProducesResponseType(typeof(BaseResponse<GetNotificationsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotifications()
    {
        return Ok(await mediator.Send(new GetNotificationsQuery()));
    }
}