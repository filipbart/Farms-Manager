using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Employees.Payslips;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Employees.Payslips;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[Route("api/employees-payslips")]
public class EmployeesPayslipsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca farmy dla rozliczenia
    /// </summary>
    /// <returns></returns>
    [HttpGet("farms")]
    [ProducesResponseType(typeof(BaseResponse<GetPayslipsFarmsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPayslipsFarms()
    {
        return Ok(await mediator.Send(new GetPayslipsFarmsQuery()));
    }

    /// <summary>
    /// Zwraca rozliczenia wypłat
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetEmployeePayslipsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPayslips([FromQuery] GetEmployeePayslipsQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetEmployeePayslipsQuery(filters)));
    }

    /// <summary>
    /// Dodaje nowe rozliczenia wypłat
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddEmployeePayslip(AddEmployeePayslipCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktualizuje rolizeczenie wypłaty
    /// </summary>
    /// <param name="employeePayslipId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update/{employeePayslipId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmployeePayslip([FromRoute] Guid employeePayslipId,
        UpdateEmployeePayslipData data)
    {
        return Ok(await mediator.Send(new UpdateEmployeePayslipCommand(employeePayslipId, data)));
    }

    /// <summary>
    /// Usuwa rolizeczenie wypłaty
    /// </summary>
    /// <param name="employeePayslipId"></param>
    /// <returns></returns>
    [HttpDelete("delete/{employeePayslipId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteEmployeePayslip([FromRoute] Guid employeePayslipId)
    {
        return Ok(await mediator.Send(new DeleteEmployeePayslipCommand(employeePayslipId)));
    }
}