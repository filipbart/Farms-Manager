using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Auth;
using FarmsManager.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class AuthController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Autoryzacja użytkownika
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("authenticate")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BaseResponse<AuthenticateCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Authenticate(AuthenticateCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Odświeża token JWT użytkownika.
    /// </summary>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(BaseResponse<RefreshTokenCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken()
    {
        return Ok(await mediator.Send(new RefreshTokenCommand()));
    }

    /// <summary>
    /// Usuwa token JWT z aktywnych sesji użytkownika.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(LogoutCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}