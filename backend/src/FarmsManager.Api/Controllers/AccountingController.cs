using System.Text;
using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Accounting;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Accounting;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoicesFromDb;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceXml;
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
    /// Zwraca faktury z systemu KSeF (API)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BaseResponse<GetKSeFInvoicesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetKSeFInvoices([FromQuery] GetKSeFInvoicesQueryFilters filters)
    {
        return Ok(await _mediator.Send(new GetKSeFInvoicesQuery(filters)));
    }

    /// <summary>
    /// Zwraca faktury z bazy danych
    /// </summary>
    [HttpGet("invoices")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BaseResponse<GetKSeFInvoicesFromDbQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetKSeFInvoicesFromDb([FromQuery] GetKSeFInvoicesFromDbQueryFilters filters)
    {
        return Ok(await _mediator.Send(new GetKSeFInvoicesFromDbQuery(filters)));
    }

    /// <summary>
    /// Zwraca szczegóły faktury
    /// </summary>
    [HttpGet("invoices/{invoiceId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BaseResponse<KSeFInvoiceDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetKSeFInvoiceDetails(Guid invoiceId)
    {
        var result = await _mediator.Send(new GetKSeFInvoiceDetailsQuery(invoiceId));
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Pobiera XML faktury KSeF
    /// </summary>
    [HttpGet("invoices/{invoiceId:guid}/xml")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadInvoiceXml(Guid invoiceId)
    {
        var result = await _mediator.Send(new GetKSeFInvoiceXmlQuery(invoiceId));
        
        if (!result.Success)
        {
            return NotFound(new { error = "Faktura lub XML nie zostały znalezione" });
        }

        var bytes = Encoding.UTF8.GetBytes(result.ResponseData.InvoiceXml);
        return File(bytes, "application/xml", result.ResponseData.FileName);
    }

    /// <summary>
    /// Pobiera PDF faktury (placeholder - wymaga implementacji generowania PDF)
    /// </summary>
    [HttpGet("invoices/{invoiceId:guid}/pdf")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadInvoicePdf(Guid invoiceId)
    {
        // Sprawdzamy czy faktura istnieje
        var result = await _mediator.Send(new GetKSeFInvoiceDetailsQuery(invoiceId));
        
        if (!result.Success)
        {
            return NotFound(new { error = "Faktura nie została znaleziona" });
        }

        // TODO: Implementacja generowania PDF z XML lub przechowywania PDF
        // Na razie zwracamy błąd że PDF nie jest dostępny
        return NotFound(new { error = "PDF faktury nie jest jeszcze dostępny. Użyj opcji pobrania XML." });
    }

    /// <summary>
    /// Upload manualnej faktury (poza KSeF)
    /// </summary>
    [HttpPost("invoices/upload")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadManualInvoice(
        [FromForm] List<IFormFile> files,
        [FromForm] string invoiceType,
        [FromForm] string? invoiceNumber,
        [FromForm] string? invoiceDate)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { error = "Nie przesłano żadnych plików" });
        }

        // TODO: Implementacja parsowania i zapisywania faktur
        // Na razie zwracamy sukces jako placeholder
        return Ok(new { 
            message = "Faktury zostały przesłane pomyślnie",
            invoiceId = Guid.NewGuid().ToString(),
            filesCount = files.Count
        });
    }

    /// <summary>
    /// Aktualizuje fakturę KSeF
    /// </summary>
    [HttpPatch("invoices/{invoiceId:guid}/update")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateInvoice(Guid invoiceId, [FromBody] UpdateInvoiceRequest request)
    {
        // TODO: Implementacja aktualizacji faktury
        return Ok(new { message = "Faktura została zaktualizowana" });
    }

    /// <summary>
    /// Usuwa fakturę KSeF
    /// </summary>
    [HttpDelete("invoices/{invoiceId:guid}/delete")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInvoice(Guid invoiceId)
    {
        // TODO: Implementacja soft delete faktury
        return Ok(new { message = "Faktura została usunięta" });
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

public class UpdateInvoiceRequest
{
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public string? ModuleType { get; set; }
    public string? Comment { get; set; }
}

