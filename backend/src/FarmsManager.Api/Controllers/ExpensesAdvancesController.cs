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