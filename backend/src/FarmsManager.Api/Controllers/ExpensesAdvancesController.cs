using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Expenses.Advances;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Expenses.Advances;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[Route("api/expenses/advances")]
public class ExpensesAdvancesController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca ewidencje zaliczek dla danego pracownika
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("{employeeId:guid}")]
    [ProducesResponseType(typeof(BaseResponse<GetExpensesAdvancesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpensesAdvances([FromRoute] Guid employeeId,
        [FromQuery] GetExpensesAdvancesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetExpensesAdvancesQuery(employeeId, filters)));
    }

    /// <summary>
    /// Dodaje ewidencje zaliczek dla pracownika
    /// </summary>
    /// <param name="employeeId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("{employeeId:guid}/add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddExpenseAdvance([FromRoute] Guid employeeId,
        [FromForm] AddExpenseAdvanceData data)
    {
        return Ok(await mediator.Send(new AddExpenseAdvanceCommand(employeeId, data)));
    }

    /// <summary>
    /// Aktualizuje ewidencje zaliczek pracownika
    /// </summary>
    /// <param name="expenseAdvanceId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("{expenseAdvanceId:guid}/update")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateExpenseAdvance([FromRoute] Guid expenseAdvanceId,
        [FromForm] UpdateExpenseAdvanceData data)
    {
        return Ok(await mediator.Send(new UpdateExpenseAdvanceCommand(expenseAdvanceId, data)));
    }

    /// <summary>
    /// Usuwa ewidencje zaliczek pracownika
    /// </summary>
    /// <param name="expenseAdvanceId"></param>
    /// <returns></returns>
    [HttpDelete("{expenseAdvanceId:guid}/delete")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteExpenseAdvance([FromRoute] Guid expenseAdvanceId)
    {
        return Ok(await mediator.Send(new DeleteExpenseAdvanceCommand(expenseAdvanceId)));
    }

    /// <summary>
    /// Zwraca kategorie ewidencji zaliczek
    /// </summary>
    /// <returns></returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(BaseResponse<List<ExpenseAdvanceCategoryRow>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await mediator.Send(new GetExpensesAdvancesCategoriesQuery()));
    }

    /// <summary>
    /// Dodaje nową kategorię ewidencji zaliczek
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("categories/add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddAdvanceCategory(List<AddExpenseAdvanceCategoryField> data)
    {
        return Ok(await mediator.Send(new AddExpenseAdvanceCategoryCommand(data)));
    }

    /// <summary>
    /// Usuwa kategorię ewidencji zaliczek
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpDelete("categories/delete/{categoryId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAdvanceCategory(Guid categoryId)
    {
        return Ok(await mediator.Send(new DeleteAdvanceCategoryCommand(categoryId)));
    }
}