using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Expenses.Types;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Expenses.Types;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class ExpensesController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca typy wydatków
    /// </summary>
    /// <returns></returns>
    [HttpGet("types")]
    [ProducesResponseType(typeof(BaseResponse<GetExpensesTypesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpensesTypes()
    {
        return Ok(await mediator.Send(new GetExpensesTypesQuery()));
    }

    /// <summary>
    /// Dodaje nowe typy wydatków
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add-type")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNewExpensesTypes(AddNewExpensesTypesCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa typ wydatku
    /// </summary>
    /// <param name="expenseTypeId"></param>
    /// <returns></returns>
    [HttpDelete("delete-type/{expenseTypeId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteExpensesType([FromRoute] Guid expenseTypeId)
    {
        return Ok(await mediator.Send(new DeleteExpenseTypeCommand(expenseTypeId)));
    }
}