using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Feeds.Deliveries;
using FarmsManager.Application.Commands.Feeds.Names;
using FarmsManager.Application.Commands.Feeds.Prices;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Feeds;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Feeds.View)]
public class FeedsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca słowniki filtrów
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetFeedsDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionaries()
    {
        return Ok(await mediator.Send(new GetFeedsDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca nazwy pasz
    /// </summary>
    /// <returns></returns>
    [HttpGet("names")]
    [ProducesResponseType(typeof(BaseResponse<GetFeedsNamesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFeedsNames()
    {
        return Ok(await mediator.Send(new GetFeedsNamesQuery()));
    }

    /// <summary>
    /// Dodaje pole dodatkowe dla sprzedaży
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("names/add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFeedName(AddFeedNameCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa pole dodatkowe dla sprzedaży
    /// </summary>
    /// <param name="feedNameId"></param>
    /// <returns></returns>
    [HttpDelete("names/delete/{feedNameId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSaleFieldExtra([FromRoute] Guid feedNameId)
    {
        return Ok(await mediator.Send(new DeleteFeedNameCommand(feedNameId)));
    }

    /// <summary>
    /// Zwraca ceny pasz
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("prices")]
    [HasPermission(AppPermissions.Feeds.PricesView)]
    [ProducesResponseType(typeof(BaseResponse<GetFeedsPricesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFeedsPrices([FromQuery] GetFeedsPricesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetFeedsPricesQuery(filters)));
    }

    /// <summary>
    /// Dodaje nową cenę paszy
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add-price")]
    [HasPermission(AppPermissions.Feeds.PricesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFeedPrice(AddFeedPriceCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktualizuje dane ceny paszy
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update-price/{id:guid}")]
    [HasPermission(AppPermissions.Feeds.PricesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFeedPrice([FromRoute] Guid id, [FromBody] UpdateFeedPriceCommandDto data)
    {
        return Ok(await mediator.Send(new UpdateFeedPriceCommand(id, data)));
    }

    /// <summary>
    /// Usuwa cene paszy
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete-price/{id:guid}")]
    [HasPermission(AppPermissions.Feeds.PricesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFeedPrice([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteFeedPriceCommand(id)));
    }

    /// <summary>
    /// Zapisuje pliki 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("upload-deliveries")]
    [Consumes("multipart/form-data")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(BaseResponse<UploadDeliveriesFilesCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadDeliveriesInvoices([FromForm] UploadDeliveriesFilesCommandDto dto)
    {
        return Ok(await mediator.Send(new UploadDeliveriesFilesCommand(dto)));
    }

    /// <summary>
    /// Zapisuje dane z faktury
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("save-invoice")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SaveInvoiceData(SaveFeedInvoiceDataCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Zwraca faktury dostaw pasz
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("deliveries")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(BaseResponse<GetFeedsDeliveriesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFeedsDeliveries([FromQuery] GetFeedsDeliveriesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetFeedsDeliveriesQuery(filters)));
    }

    /// <summary>
    /// Zwraca plik faktury
    /// </summary>
    /// <param name="feedDeliveryId"></param>
    /// <returns></returns>
    [HttpGet("download-file/{feedDeliveryId:guid}")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFeedInvoiceFile([FromRoute] Guid feedDeliveryId)
    {
        var file = await mediator.Send(new GetFeedDeliveryFileQuery(feedDeliveryId));

        return file is null ? NoContent() : File(file.Data, file.ContentType, file.FileName);
    }

    /// <summary>
    /// Aktualizuje dane faktury dostawy paszy
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update-delivery/{id:guid}")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFeedDelivery([FromRoute] Guid id,
        [FromBody] UpdateFeedDeliveryCommandDto data)
    {
        return Ok(await mediator.Send(new UpdateFeedDeliveryCommand(id, data)));
    }

    /// <summary>
    /// Usuwa cene paszy
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete-delivery/{id:guid}")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFeedDelivery([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteFeedDeliveryCommand(id)));
    }

    /// <summary>
    /// Zwraca dokument płatności wybranych faktur 
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="comment"></param>
    /// <returns></returns>
    [HttpGet("payment-file")]
    [HasPermission(AppPermissions.Feeds.PaymentsView)]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFeedPaymentFile([FromQuery] List<Guid> ids, [FromQuery] string? comment)
    {
        var file = await mediator.Send(new GetFeedDeliveryPaymentFileQuery(ids, comment));

        return file is null ? NoContent() : File(file.Data, file.ContentType, file.FileName);
    }

    /// <summary>
    /// Usuwa przelew
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete-payment/{id:guid}")]
    [HasPermission(AppPermissions.Feeds.PaymentsView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFeedPayment([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteFeedPaymentCommand(id)));
    }

    /// <summary>
    /// Zwraca wszystkie przelewy dla dostaw
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("payments")]
    [HasPermission(AppPermissions.Feeds.PaymentsView)]
    [ProducesResponseType(typeof(BaseResponse<GetFeedsPaymentsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFeedsPayments([FromQuery] GetFeedsPaymentsQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetFeedsPaymentsQuery(filters)));
    }

    /// <summary>
    /// Oznacza przelew jako zrealizowany
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("mark-payment-completed/{id:guid}")]
    [HasPermission(AppPermissions.Feeds.PaymentsView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkPaymentAsCompleted([FromRoute] Guid id,
        [FromBody] MarkPaymentAsCompletedCommandDto data)
    {
        return Ok(await mediator.Send(new MarkPaymentAsCompletedCommand(id, data)));
    }

    /// <summary>
    /// Dodaje nową korektę
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("add-correction")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddFeedInvoiceCorrection([FromForm] AddFeedInvoiceCorrectionCommandDto dto)
    {
        return Ok(await mediator.Send(new AddFeedInvoiceCorrectionCommand(dto)));
    }

    /// <summary>
    /// Usuwa korektę
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete-correction/{id:guid}")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFeedCorrection([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteFeedCorrectionCommand(id)));
    }

    /// <summary>
    /// Aktualizuje korektę
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPatch("update-correction")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFeedCorrection([FromBody] UpdateFeedCorrectionDto dto)
    {
        return Ok(await mediator.Send(new UpdateFeedCorrectionCommand(dto)));
    }

    /// <summary>
    /// Pobiera wiele faktur pasz jako archiwum ZIP
    /// </summary>
    /// <param name="deliveryIds">Lista ID zwykłych dostaw pasz</param>
    /// <param name="correctionFilePaths">Lista ścieżek do plików faktur korygujących</param>
    /// <returns></returns>
    [HttpGet("download-invoices-zip")]
    [HasPermission(AppPermissions.Feeds.DeliveriesView)]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DownloadInvoicesAsZip(
        [FromQuery] List<Guid>? deliveryIds,
        [FromQuery] List<string>? correctionFilePaths)
    {
        if ((deliveryIds is null || deliveryIds.Count == 0) &&
            (correctionFilePaths is null || correctionFilePaths.Count == 0))
        {
            return BadRequest("Brak danych do pobrania");
        }

        var zipData = await mediator.Send(new DownloadFeedInvoicesAsZipQuery(deliveryIds, correctionFilePaths));
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        return File(zipData, "application/zip", $"faktury_pasz_{timestamp}.zip");
    }
}