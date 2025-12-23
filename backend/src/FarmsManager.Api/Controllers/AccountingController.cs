using System.Text;
using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Accounting;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Accounting;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoicesFromDb;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoicePdf;
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
    /// Pobiera PDF faktury wygenerowany z danych faktury
    /// </summary>
    [HttpGet("invoices/{invoiceId:guid}/pdf")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadInvoicePdf(Guid invoiceId)
    {
        try
        {
            var result = await _mediator.Send(new GetKSeFInvoicePdfQuery(invoiceId));
            return File(result.Data, result.ContentType, result.FileName);
        }
        catch (Exception ex) when (ex.Message.Contains("nie została znaleziona"))
        {
            return NotFound(new { error = "Faktura nie została znaleziona" });
        }
    }

    /// <summary>
    /// Upload faktur z zaczytywaniem danych przez AI
    /// </summary>
    [HttpPost("invoices/upload")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BaseResponse<UploadAccountingInvoicesCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadInvoices(
        [FromForm] List<IFormFile> files,
        [FromForm] string invoiceType)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { error = "Nie przesłano żadnych plików" });
        }

        var result = await _mediator.Send(new UploadAccountingInvoicesCommand(
            new UploadAccountingInvoicesCommandDto
            {
                Files = files,
                InvoiceType = invoiceType
            }));

        return Ok(result);
    }

    /// <summary>
    /// Zapisuje fakturę po zaczytaniu przez AI
    /// </summary>
    [HttpPost("invoices/save")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(BaseResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SaveInvoice([FromBody] SaveAccountingInvoiceDto data)
    {
        var result = await _mediator.Send(new SaveAccountingInvoiceCommand(data));
        return Ok(result);
    }

    /// <summary>
    /// Aktualizuje fakturę KSeF
    /// </summary>
    [HttpPatch("invoices/{invoiceId:guid}/update")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateInvoice(Guid invoiceId, [FromBody] UpdateInvoiceRequest request)
    {
        var result = await _mediator.Send(new UpdateKSeFInvoiceCommand(invoiceId, new UpdateKSeFInvoiceDto
        {
            Status = request.Status,
            PaymentStatus = request.PaymentStatus,
            ModuleType = request.ModuleType,
            VatDeductionType = request.VatDeductionType,
            Comment = request.Comment,
            FarmId = request.FarmId,
            CycleId = request.CycleId,
            AssignedUserId = request.AssignedUserId,
            RelatedInvoiceNumber = request.RelatedInvoiceNumber
        }));

        return Ok(result);
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

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Pobiera listę faktur możliwych do powiązania z daną fakturą
    /// </summary>
    [HttpGet("invoices/{invoiceId:guid}/linkable")]
    [ProducesResponseType(typeof(BaseResponse<List<LinkableInvoiceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLinkableInvoices(Guid invoiceId, [FromQuery] string searchPhrase = null, [FromQuery] int limit = 20)
    {
        var filters = new GetLinkableInvoicesFilters
        {
            SourceInvoiceId = invoiceId,
            SearchPhrase = searchPhrase,
            Limit = limit
        };
        return Ok(await _mediator.Send(new GetLinkableInvoicesQuery(filters)));
    }

    /// <summary>
    /// Tworzy powiązania między fakturami
    /// </summary>
    [HttpPost("invoices/link")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LinkInvoices([FromBody] LinkInvoicesDto request)
    {
        return Ok(await _mediator.Send(new LinkInvoicesCommand(request)));
    }

    /// <summary>
    /// Akceptuje brak powiązania dla faktury
    /// </summary>
    [HttpPost("invoices/{invoiceId:guid}/accept-no-linking")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AcceptNoLinking(Guid invoiceId)
    {
        return Ok(await _mediator.Send(new AcceptNoLinkingCommand(invoiceId)));
    }

    /// <summary>
    /// Odkłada przypomnienie o powiązaniu faktury
    /// </summary>
    [HttpPost("invoices/{invoiceId:guid}/postpone-linking")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostponeLinkingReminder(Guid invoiceId, [FromQuery] int days = 3)
    {
        return Ok(await _mediator.Send(new PostponeLinkingReminderCommand(invoiceId, days)));
    }
}

public class UpdateInvoiceRequest
{
    public KSeFInvoiceStatus? Status { get; set; }
    public KSeFPaymentStatus? PaymentStatus { get; set; }
    public ModuleType? ModuleType { get; set; }
    public KSeFVatDeductionType? VatDeductionType { get; set; }
    public string? Comment { get; set; }
    public Guid? FarmId { get; set; }
    public Guid? CycleId { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string? RelatedInvoiceNumber { get; set; }
}