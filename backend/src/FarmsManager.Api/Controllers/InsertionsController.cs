using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Insertions;
using FarmsManager.Application.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class InsertionsController(IMediator mediator) : BaseController
{

    /// <summary>
    /// Dodaje nowy cykl
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add-cycle")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNewCycle(AddNewCycleCommand command)
    {
        return Ok(await mediator.Send(command));
    }
}