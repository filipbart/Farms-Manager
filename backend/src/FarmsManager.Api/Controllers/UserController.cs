using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.User;
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

    /// <summary>
    /// Aktualizuje dane zalogowanego użytkownika
    /// </summary>
    [HttpPatch("me")] 
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateMeData data)
    {
        return Ok(await mediator.Send(new UpdateMeCommand(data)));
    }

    /// <summary>
    /// Aktualizuje awatar użytkownika
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("avatar")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateAvatar([FromForm] UpdateUserAvatarCommand command)
    {
        return Ok(await mediator.Send(command));
    }
}