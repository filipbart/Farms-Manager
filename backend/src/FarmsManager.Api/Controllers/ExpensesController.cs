using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Expenses.Contractors;
using FarmsManager.Application.Commands.Expenses.Production;
using FarmsManager.Application.Commands.Expenses.Types;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Expenses.Contractors;
using FarmsManager.Application.Queries.Expenses.Productions;
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

    /// <summary>
    /// Zwraca kontrahentów dla kosztów
    /// </summary>
    /// <returns></returns>
    [HttpGet("contractors")]
    [ProducesResponseType(typeof(BaseResponse<GetExpensesContractorsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpensesContractors()
    {
        return Ok(await mediator.Send(new GetExpensesContractorsQuery()));
    }

    /// <summary>
    /// Dodaje nowego kontrahenta
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("add-contractor")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddExpenseContractor(AddExpenseContractorDto data)
    {
        return Ok(await mediator.Send(new AddExpenseContractorCommand(data)));
    }

    /// <summary>
    /// Usuwa kontrahenta
    /// </summary>
    /// <param name="expenseContractorId"></param>
    /// <returns></returns>
    [HttpDelete("delete-contractor/{expenseContractorId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteExpenseContractor([FromRoute] Guid expenseContractorId)
    {
        return Ok(await mediator.Send(new DeleteExpenseContractorCommand(expenseContractorId)));
    }

    /// <summary>
    /// Aktualizuje dane kontrahenta
    /// </summary>
    /// <param name="expenseContractorId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update-contractor/{expenseContractorId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateExpenseContractor([FromRoute] Guid expenseContractorId,
        AddExpenseContractorDto data)
    {
        return Ok(await mediator.Send(new UpdateExpenseContractorCommand(expenseContractorId, data)));
    }

    /// <summary>
    /// Zwraca słownik filtrów dla kosztów produkcyjnych
    /// </summary>
    /// <returns></returns>
    [HttpGet("productions/dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetExpenseProductionDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpenseProductionDictionary()
    {
        return Ok(await mediator.Send(new GetExpenseProductionDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca koszty produkcyjne
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("productions")]
    [ProducesResponseType(typeof(BaseResponse<GetExpensesProductionQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpensesProduction([FromQuery] GetExpensesProductionsFilters filters)
    {
        return Ok(await mediator.Send(new GetExpensesProductionQuery(filters)));
    }

    /// <summary>
    /// Dodaje nową fakturę kosztów ręcznie
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("productions/add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddExpenseProduction([FromForm] AddExpenseProductionData data)
    {
        return Ok(await mediator.Send(new AddExpenseProductionCommand(data)));
    }

    /// <summary>
    /// Aktualizuje dane kosztów produkcyjnych
    /// </summary>
    /// <param name="expenseProductionId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("productions/update/{expenseProductionId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateExpenseProduction([FromRoute] Guid expenseProductionId, UpdateExpenseProductionData data)
    {
        return Ok(await mediator.Send(new UpdateExpenseProductionCommand(expenseProductionId, data)));
    }

    /// <summary>
    /// Usuwa dane kosztów produkcyjnych
    /// </summary>
    /// <param name="expenseProductionId"></param>
    /// <returns></returns>
    [HttpDelete("productions/delete/{expenseProductionId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteExpenseProduction([FromRoute] Guid expenseProductionId)
    {
        return Ok(await mediator.Send(new DeleteExpenseProductionCommand(expenseProductionId)));
    }
    
    
    [HttpPost("productions/upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(BaseResponse<UploadExpensesInvoicesCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadExpenseInvoices([FromForm] UploadExpensesInvoicesDto dto)
    {
        return Ok(await mediator.Send(new UploadExpensesInvoicesCommand(dto)));
    }
}