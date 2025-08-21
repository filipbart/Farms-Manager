using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Insertions;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Insertions;
using FarmsManager.Application.Queries.Insertions.Dictionary;
using FarmsManager.Application.Queries.Insertions.Henhouses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Insertions.View)]
public class InsertionsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca wstawienia według podanych filtrów
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetInsertionsQueryResponse>), StatusCodes.Status200OK)]
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
    [ProducesResponseType(typeof(BaseResponse<AddNewInsertionCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNewInsertion(AddNewInsertionCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktualizuje dane wstawienia
    /// </summary>
    /// <param name="id"></param>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPatch("update/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateInsertion([FromRoute] Guid id, [FromBody] UpdateInsertionCommandDto payload)
    {
        return Ok(await mediator.Send(new UpdateInsertionCommand(id, payload)));
    }

    /// <summary>
    /// Usuwa wstawienie
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteInsertion([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteInsertionCommand(id)));
    }

    /// <summary>
    /// Zwraca słowniki filtrów
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetInsertionDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionaries()
    {
        return Ok(await mediator.Send(new GetInsertionDictionaryQuery()));
    }

    /// <summary>
    /// Oznacza wstawienia jako zgłoszone do WIOŚ
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("mark-reported-to-wios")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAsReportedToWios(MarkInsertionAsReportedToWiosCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Zwraca dostępne kurniki dla wstawienia
    /// </summary>
    /// <param name="farmId"></param>
    /// <returns></returns>
    [HttpGet("available-henhouses")]
    [ProducesResponseType(typeof(BaseResponse<GetInsertionDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAvailableHenhouses([FromQuery] Guid farmId)
    {
        return Ok(await mediator.Send(new GetAvailableHenhousesQuery(farmId)));
    }

    /// <summary>
    /// Wysyła wstawienie do systemu IRZplus
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("send-to-irz")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendToIrzPlus(SendInsertionToIrzCommand command)
    {
        return Ok(await mediator.Send(command));
    }
}