using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Insertions;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Insertions;
using FarmsManager.Application.Queries.Insertions.Dictionary;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class InsertionsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca wstawienia według podantych filtrów
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInsertions([FromQuery] GetInsertionsQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetInsertionsQuery(filters)));
    }

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

    /// <summary>
    /// Dodaje nowe wstawienie
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNewInsertion(AddNewInsertionCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Zwraca słowniki filtrów
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionaries()
    {
        return Ok(await mediator.Send(new GetDictionaryQuery()));
    }
}