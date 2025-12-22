using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Accounting.InvoiceAssignmentRules;
using FarmsManager.Application.Commands.Settings;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Accounting.InvoiceAssignmentRules;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class SettingsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zapisuje dane logowania do IRZplus
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("save-irzplus-credentials")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SaveIrzplusCredentials(SaveIrzplusCredentialsCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    #region Invoice Assignment Rules

    /// <summary>
    /// Pobiera listę reguł przypisywania faktur
    /// </summary>
    [HttpGet("invoice-assignment-rules")]
    [ProducesResponseType(typeof(BaseResponse<List<InvoiceAssignmentRuleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInvoiceAssignmentRules()
    {
        return Ok(await mediator.Send(new GetInvoiceAssignmentRulesQuery()));
    }

    /// <summary>
    /// Tworzy nową regułę przypisywania faktur
    /// </summary>
    [HttpPost("invoice-assignment-rules")]
    [ProducesResponseType(typeof(BaseResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateInvoiceAssignmentRule([FromBody] CreateInvoiceAssignmentRuleDto data)
    {
        return Ok(await mediator.Send(new CreateInvoiceAssignmentRuleCommand(data)));
    }

    /// <summary>
    /// Aktualizuje regułę przypisywania faktur
    /// </summary>
    [HttpPut("invoice-assignment-rules/{ruleId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateInvoiceAssignmentRule(Guid ruleId, [FromBody] UpdateInvoiceAssignmentRuleDto data)
    {
        return Ok(await mediator.Send(new UpdateInvoiceAssignmentRuleCommand(ruleId, data)));
    }

    /// <summary>
    /// Usuwa regułę przypisywania faktur
    /// </summary>
    [HttpDelete("invoice-assignment-rules/{ruleId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteInvoiceAssignmentRule(Guid ruleId)
    {
        return Ok(await mediator.Send(new DeleteInvoiceAssignmentRuleCommand(ruleId)));
    }

    /// <summary>
    /// Zmienia kolejność reguł przypisywania faktur
    /// </summary>
    [HttpPost("invoice-assignment-rules/reorder")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReorderInvoiceAssignmentRules([FromBody] List<Guid> orderedRuleIds)
    {
        return Ok(await mediator.Send(new ReorderInvoiceAssignmentRulesCommand(orderedRuleIds)));
    }

    #endregion
}