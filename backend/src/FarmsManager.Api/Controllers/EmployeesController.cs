using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Employees;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Employees.ListView)]
public class EmployeesController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca słownik dla listy kadr
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetEmployeesDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionary()
    {
        return Ok(await mediator.Send(new GetEmployeesDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca listę kadr
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetEmployeesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEmployees([FromQuery] GetEmployeesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetEmployeesQuery(filters)));
    }

    /// <summary>
    /// Dodaje nowego pracownika
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddEmployee(AddEmployeeData data)
    {
        return Ok(await mediator.Send(new AddEmployeeCommand(data)));
    }

    /// <summary>
    /// Usuwa pracownika
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    [HttpDelete("{employeeId:guid}/delete")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteEmployee([FromRoute] Guid employeeId)
    {
        return Ok(await mediator.Send(new DeleteEmployeeCommand(employeeId)));
    }

    /// <summary>
    /// Zwraca szczegóły pracownika
    /// </summary>
    /// <param name="employeeId"></param>
    /// <returns></returns>
    [HttpGet("{employeeId:guid}")]
    [ProducesResponseType(typeof(BaseResponse<GetEmployeesQueryHandler>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEmployeeDetails([FromRoute] Guid employeeId)
    {
        return Ok(await mediator.Send(new GetEmployeeDetailsQuery(employeeId)));
    }

    /// <summary>
    /// Aktualizuje pracownika
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("{employeeId:guid}/update")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmployee([FromRoute] Guid employeeId, UpdateEmployeeData data)
    {
        return Ok(await mediator.Send(new UpdateEmployeeCommand(employeeId, data)));
    }

    /// <summary>
    /// Dodaje pliki do pracownika
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost("{employeeId:guid}/upload-files")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadEmployeeFiles([FromRoute] Guid employeeId,
        [FromForm] IFormFileCollection files)
    {
        return Ok(await mediator.Send(new UploadEmployeeFilesCommand(employeeId, files)));
    }

    /// <summary>
    /// Usuwa plik pracownika
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="fileId"></param>
    /// <returns></returns>
    [HttpDelete("{employeeId:guid}/delete-file/{fileId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteEmployeeFile([FromRoute] Guid employeeId, [FromRoute] Guid fileId)
    {
        return Ok(await mediator.Send(new DeleteEmployeeFileCommand(employeeId, fileId)));
    }

    /// <summary>
    /// Dodaje przypomnienie do uzytkownika
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("{employeeId:guid}/add-reminder")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddEmployeeReminder([FromRoute] Guid employeeId, AddEmployeeReminderData data)
    {
        return Ok(await mediator.Send(new AddEmployeeReminderCommand(employeeId, data)));
    }

    /// <summary>
    /// Usuwa przypomnienie uzytkownika
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="reminderId"></param>
    /// <returns></returns>
    [HttpDelete("{employeeId:guid}/delete-reminder/{reminderId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteEmployeeReminder([FromRoute] Guid employeeId, Guid reminderId)
    {
        return Ok(await mediator.Send(new DeleteEmployeeReminderCommand(employeeId, reminderId)));
    }
}