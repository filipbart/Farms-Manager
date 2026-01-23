using System.Text;
using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Accounting;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Accounting;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceDetails;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoicesFromDb;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoicePdf;
using FarmsManager.Application.Queries.Accounting.GetKSeFInvoiceXml;
using FarmsManager.Application.Queries.Accounting.GetInvoicesZip;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Accounting.Manage)]
public class AccountingController : BaseController
{
    private readonly IMediator _mediator;

    public AccountingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Zwraca faktury z systemu KSeF (API)
    /// </summary>
    [HttpGet]
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
    [ProducesResponseType(typeof(BaseResponse<UploadAccountingInvoicesCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadInvoices(
        [FromForm] List<IFormFile> files,
        [FromForm] string invoiceType,
        [FromForm] string paymentStatus,
        [FromForm] string? moduleType,
        [FromForm] DateOnly? paymentDate)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { error = "Nie przesłano żadnych plików" });
        }

        var result = await _mediator.Send(new UploadAccountingInvoicesCommand(
            new UploadAccountingInvoicesCommandDto
            {
                Files = files,
                InvoiceType = invoiceType,
                PaymentStatus = paymentStatus,
                ModuleType = moduleType,
                PaymentDate = paymentDate
            }));

        return Ok(result);
    }

    /// <summary>
    /// Upload faktur KSeF z plików XML (bez AI, bezpośredni import)
    /// </summary>
    [HttpPost("invoices/upload-xml")]
    [ProducesResponseType(typeof(BaseResponse<UploadKSeFXmlInvoicesCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadKSeFXmlInvoices(
        [FromForm] List<IFormFile> files,
        [FromForm] string invoiceType,
        [FromForm] string paymentStatus,
        [FromForm] DateOnly? paymentDate)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new { error = "Nie przesłano żadnych plików XML" });
        }

        var result = await _mediator.Send(new UploadKSeFXmlInvoicesCommand(
            new UploadKSeFXmlInvoicesCommandDto
            {
                Files = files,
                InvoiceType = invoiceType,
                PaymentStatus = paymentStatus,
                PaymentDate = paymentDate
            }));

        return Ok(result);
    }

    /// <summary>
    /// Usuwa wszystkie faktury KSeF (tylko do testów!)
    /// </summary>
    [HttpDelete("invoices/delete-all")]
    [ProducesResponseType(typeof(BaseResponse<DeleteAllKSeFInvoicesCommandResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAllInvoices()
    {
        var result = await _mediator.Send(new DeleteAllKSeFInvoicesCommand());
        return Ok(result);
    }

    /// <summary>
    /// Zapisuje fakturę po zaczytaniu przez AI
    /// </summary>
    [HttpPost("invoices/save")]
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
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateInvoice(Guid invoiceId, [FromBody] UpdateKSeFInvoiceDto request)
    {
        var result = await _mediator.Send(new UpdateKSeFInvoiceCommand(invoiceId, request));
        return Ok(result);
    }

    /// <summary>
    /// Usuwa fakturę KSeF
    /// </summary>
    [HttpDelete("invoices/{invoiceId:guid}/delete")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteInvoice(Guid invoiceId)
    {
        var result = await _mediator.Send(new DeleteKSeFInvoiceCommand(invoiceId));
        return Ok(result);
    }

    /// <summary>
    /// Wstrzymuje fakturę i przypisuje ją do innego pracownika (nie zmienia statusu)
    /// </summary>
    [HttpPost("invoices/{invoiceId:guid}/hold")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HoldInvoice(Guid invoiceId, [FromBody] HoldKSeFInvoiceDto request)
    {
        var result = await _mediator.Send(new HoldKSeFInvoiceCommand(invoiceId, request));
        return Ok(result);
    }

    /// <summary>
    /// Pobiera pliki faktur jako ZIP
    /// </summary>
    [HttpPost("invoices/download-zip")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DownloadInvoicesZip([FromBody] List<Guid> invoiceIds)
    {
        var result = await _mediator.Send(new GetInvoicesZipQuery(invoiceIds));
        return File(result.ZipData, "application/zip", result.FileName);
    }

    [HttpPost("send-invoice")]
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
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> TriggerKSeFSynchronization()
    {
        var result = await _mediator.Send(new SyncKSeFInvoicesCommand());
        return Ok(result);
    }

    /// <summary>
    /// Pobiera listę faktur możliwych do powiązania z daną fakturą
    /// </summary>
    [HttpGet("invoices/{invoiceId:guid}/linkable")]
    [ProducesResponseType(typeof(BaseResponse<List<LinkableInvoiceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLinkableInvoices(Guid invoiceId, [FromQuery] string? searchPhrase = null,
        [FromQuery] int limit = 20)
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

    /// <summary>
    /// Tworzy encję w module na podstawie faktury KSeF
    /// </summary>
    [HttpPost("invoices/{invoiceId:guid}/create-module-entity")]
    [ProducesResponseType(typeof(BaseResponse<Guid?>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateModuleEntityFromInvoice(
        Guid invoiceId,
        [FromBody] CreateModuleEntityFromInvoiceRequest request)
    {
        var command = new CreateModuleEntityFromInvoiceCommand
        {
            KSeFInvoiceId = invoiceId,
            ModuleType = request.ModuleType,
            FeedData = request.FeedData,
            GasData = request.GasData,
            ExpenseData = request.ExpenseData,
            SaleData = request.SaleData
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Akceptuje fakturę i tworzy encję w odpowiednim module.
    /// Zmienia status faktury na "Accepted" i wymaga podania danych modułowych (jeśli moduł tego wymaga).
    /// </summary>
    [HttpPost("invoices/{invoiceId:guid}/accept")]
    [ProducesResponseType(typeof(BaseResponse<Guid?>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptInvoice(
        Guid invoiceId,
        [FromBody] AcceptKSeFInvoiceRequest request)
    {
        var command = new AcceptKSeFInvoiceCommand
        {
            InvoiceId = invoiceId,
            ModuleType = request.ModuleType,
            FeedData = request.FeedData,
            GasData = request.GasData,
            ExpenseData = request.ExpenseData,
            SaleData = request.SaleData
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Synchronizuje status płatności między fakturą KSeF a powiązanym modułem
    /// </summary>
    [HttpPost("invoices/{invoiceId:guid}/sync-payment-status")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SyncPaymentStatus(Guid invoiceId, [FromBody] SyncPaymentStatusRequest request)
    {
        var result =
            await _mediator.Send(new SyncPaymentStatusCommand(invoiceId, request.Direction, request.NewPaymentStatus));
        return Ok(result);
    }

    #region Attachments

    /// <summary>
    /// Przesyła załącznik do faktury
    /// </summary>
    [HttpPost("invoices/{invoiceId:guid}/attachments")]
    [ProducesResponseType(typeof(BaseResponse<InvoiceAttachmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadAttachment(Guid invoiceId, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "Nie przesłano pliku" });
        }

        var result = await _mediator.Send(new UploadInvoiceAttachmentCommand(invoiceId, file));
        return Ok(result);
    }

    /// <summary>
    /// Pobiera załącznik faktury
    /// </summary>
    [HttpGet("invoices/{invoiceId:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAttachment(Guid invoiceId, Guid attachmentId)
    {
        var result = await _mediator.Send(new DownloadInvoiceAttachmentQuery(invoiceId, attachmentId));
        return File(result.Content, result.ContentType, result.FileName);
    }

    /// <summary>
    /// Usuwa załącznik faktury
    /// </summary>
    [HttpDelete("invoices/{invoiceId:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAttachment(Guid invoiceId, Guid attachmentId)
    {
        var result = await _mediator.Send(new DeleteInvoiceAttachmentCommand(invoiceId, attachmentId));
        return Ok(result);
    }

    /// <summary>
    /// Pobiera listę załączników faktury
    /// </summary>
    [HttpGet("invoices/{invoiceId:guid}/attachments")]
    [ProducesResponseType(typeof(BaseResponse<List<InvoiceAttachmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttachments(Guid invoiceId)
    {
        var result = await _mediator.Send(new GetInvoiceAttachmentsQuery(invoiceId));
        return Ok(result);
    }

    #endregion

    #region Audit Logs

    /// <summary>
    /// Pobiera historię audytu faktury
    /// </summary>
    [HttpGet("invoices/{invoiceId:guid}/audit-logs")]
    [ProducesResponseType(typeof(BaseResponse<List<InvoiceAuditLogDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoiceAuditLogs(Guid invoiceId)
    {
        var result = await _mediator.Send(new GetInvoiceAuditLogsQuery(invoiceId));
        return Ok(result);
    }

    #endregion
}

/// <summary>
/// Request do akceptacji faktury
/// </summary>
public class AcceptKSeFInvoiceRequest
{
    public ModuleType ModuleType { get; set; }
    public CreateFeedInvoiceFromKSeFDto? FeedData { get; set; }
    public CreateGasDeliveryFromKSeFDto? GasData { get; set; }
    public CreateExpenseProductionFromKSeFDto? ExpenseData { get; set; }
    public CreateSaleInvoiceFromKSeFDto? SaleData { get; set; }
}

/// <summary>
/// DTO załącznika faktury
/// </summary>
public class InvoiceAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedByName { get; set; }
}

/// <summary>
/// DTO logu audytu faktury
/// </summary>
public class InvoiceAuditLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; }
    public string PreviousStatus { get; set; }
    public string NewStatus { get; set; }
    public string UserName { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request do synchronizacji statusu płatności
/// </summary>
public class SyncPaymentStatusRequest
{
    /// <summary>
    /// Kierunek synchronizacji: "ToAccounting" lub "FromAccounting"
    /// </summary>
    public string Direction { get; set; }

    /// <summary>
    /// Nowy status płatności (wymagany tylko dla Direction="FromAccounting")
    /// </summary>
    public string NewPaymentStatus { get; set; }
}