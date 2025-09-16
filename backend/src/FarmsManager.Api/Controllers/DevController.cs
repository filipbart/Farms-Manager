using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Dev;
using FarmsManager.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class DevController(IMediator mediator) : BaseController
{
    [AllowAnonymous]
    [HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount(CreateDevAccountCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword(ResetPasswordDevCommand command)
    {
        return Ok(await mediator.Send(command));
    }
}