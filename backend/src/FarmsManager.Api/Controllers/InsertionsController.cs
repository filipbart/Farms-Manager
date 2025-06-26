using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Insertions;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Insertions;
using FarmsManager.Application.Queries.Insertions.Dictionary;
using FarmsManager.Application.Queries.Insertions.Henhouses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
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
    [ProducesResponseType(typeof(BaseResponse<GetDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionaries()
    {
        return Ok(await mediator.Send(new GetDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca dostępne kurniki dla wstawienia
    /// </summary>
    /// <param name="farmId"></param>
    /// <returns></returns>
    [HttpGet("available-henhouses")]
    [ProducesResponseType(typeof(BaseResponse<GetDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAvailableHenhouses([FromQuery] Guid farmId)
    {
        return Ok(await mediator.Send(new GetAvailableHenhousesQuery(farmId)));
    }

    /// <summary>
    /// Wysyła wstawienie do systemu IRZ+
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("send-to-irz")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendToIrzPlus(SendToIrzCommand command)
    {
        return Ok(await mediator.Send(command));
    }
}