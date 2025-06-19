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
}