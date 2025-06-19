using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Dev;
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
}