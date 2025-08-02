using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Employees;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

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
    [ProducesResponseType(typeof(BaseResponse<GetEmployeesQueryHandler>), StatusCodes.Status200OK)]
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
    public async Task<IActionResult> DeleteEmployee(Guid employeeId)
    {
        return Ok(await mediator.Send(new DeleteEmployeeCommand(employeeId)));
    }
}