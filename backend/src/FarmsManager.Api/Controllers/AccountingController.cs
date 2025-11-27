using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Accounting;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Accounting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Accounting.Manage)]
public class AccountingController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca faktury z systemu KSeF
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BaseResponse<GetKSeFInvoicesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetKSeFInvoices([FromQuery] GetKSeFInvoicesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetKSeFInvoicesQuery(filters)));
    }
    
    [HttpPost("send-invoice")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendTestKSeFInvoice(IFormFile? file = null)
    {
        string? fileContent = null;
        if (file != null)
        {
            using var reader = new StreamReader(file.OpenReadStream());
            fileContent = await reader.ReadToEndAsync();
        }
        
        return Ok(await mediator.Send(new SendTestKSeFInvoiceCommand(fileContent)));
    }
}

