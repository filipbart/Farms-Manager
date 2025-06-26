using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Settings;
using FarmsManager.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class SettingsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zapisuje dane logowania do IRZplus
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("save-irzplus-credentials")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SaveIrzplusCredentials(SaveIrzplusCredentialsCommand command)
    {
        return Ok(await mediator.Send(command));
    }
}