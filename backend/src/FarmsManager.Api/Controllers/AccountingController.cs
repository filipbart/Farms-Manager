using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Accounting;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Accounting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Accounting.Manage)]
public class AccountingController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IKSeFSynchronizationJob _ksefSyncJob;

    public AccountingController(IMediator mediator, IKSeFSynchronizationJob ksefSyncJob)
    {
        _mediator = mediator;
        _ksefSyncJob = ksefSyncJob;
    }
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
        return Ok(await _mediator.Send(new GetKSeFInvoicesQuery(filters)));
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
        
        return Ok(await _mediator.Send(new SendTestKSeFInvoiceCommand(fileContent)));
    }

    /// <summary>
    /// Ręczne uruchomienie synchronizacji faktur z KSeF
    /// </summary>
    [HttpPost("sync-ksef-invoices")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TriggerKSeFSynchronization()
    {
        try
        {
            // Uruchomienie synchronizacji w tle (fire and forget)
            await _ksefSyncJob.ExecuteSynchronizationAsync(isManual: true);
            
            return Accepted(new { message = "KSeF synchronization has been triggered successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

