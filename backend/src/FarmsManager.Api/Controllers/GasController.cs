using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Gas.Consumptions;
using FarmsManager.Application.Commands.Gas.Deliveries;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Gas;
using FarmsManager.Application.Queries.Gas.Consumptions;
using FarmsManager.Application.Queries.Gas.Deliveries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[HasPermission(AppPermissions.Gas.View)]
public class GasController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca słownik filtrów dla dostaw gazu
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [HasPermission(AppPermissions.Gas.DeliveriesView)]
    [ProducesResponseType(typeof(BaseResponse<GetGasDeliveriesDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGasDeliveriesDictionary()
    {
        return Ok(await mediator.Send(new GetGasDeliveriesDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca dostawy gazu
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("deliveries")]
    [HasPermission(AppPermissions.Gas.DeliveriesView)]
    [ProducesResponseType(typeof(BaseResponse<GetGasDeliveriesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGasDeliveries([FromQuery] GetGasDeliveriesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetGasDeliveriesQuery(filters)));
    }

    /// <summary>
    /// Zwraca kontrahentów dostaw gazu
    /// </summary>
    /// <returns></returns>
    [HttpGet("contractors")]
    [HasPermission(AppPermissions.Gas.DeliveriesView)]
    [ProducesResponseType(typeof(BaseResponse<GetGasContractorsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGasContractors()
    {
        return Ok(await mediator.Send(new GetGasContractorsQuery()));
    }

    /// <summary>
    /// Wrzuca tymczasowo pliki faktur i je odczytuje dla dostawy gazu
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("deliveries/upload")]
    [Consumes("multipart/form-data")]
    [HasPermission(AppPermissions.Gas.DeliveriesView)]
    [ProducesResponseType(typeof(BaseResponse<UploadGasDeliveriesInvoicesCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadExpenseInvoices([FromForm] UploadGasDeliveriesInvoicesDto dto)
    {
        return Ok(await mediator.Send(new UploadGasDeliveriesInvoicesCommand(dto)));
    }

    /// <summary>
    /// Zapisuje dane faktury dostawy gazu
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("deliveries/save-invoice")]
    [HasPermission(AppPermissions.Gas.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SaveGasDeliveryInvoice(SaveGasDeliveryInvoiceCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Dodaje nową dostawę gazu
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("deliveries/add")]
    [HasPermission(AppPermissions.Gas.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddGasDelivery([FromForm] AddGasDeliveryData data)
    {
        return Ok(await mediator.Send(new AddGasDeliveryCommand(data)));
    }

    /// <summary>
    /// Aktualizuje wpis dostawy gazu
    /// </summary>
    /// <param name="gasDeliveryId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("deliveries/update/{gasDeliveryId:guid}")]
    [HasPermission(AppPermissions.Gas.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateGasDelivery([FromRoute] Guid gasDeliveryId, UpdateGasDeliveryData data)
    {
        return Ok(await mediator.Send(new UpdateGasDeliveryCommand(gasDeliveryId, data)));
    }

    /// <summary>
    /// Usuwa wpis dostawy gazu
    /// </summary>
    /// <param name="gasDeliveryId"></param>
    /// <returns></returns>
    [HttpDelete("deliveries/delete/{gasDeliveryId:guid}")]
    [HasPermission(AppPermissions.Gas.DeliveriesView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteGasDelivery([FromRoute] Guid gasDeliveryId)
    {
        return Ok(await mediator.Send(new DeleteGasDeliveryCommand(gasDeliveryId)));
    }

    /// <summary>
    /// Zwraca słownik filtrów dla zużycia gazu
    /// </summary>
    /// <returns></returns>
    [HttpGet("consumptions/dictionary")]
    [HasPermission(AppPermissions.Gas.ConsumptionsView)]
    [ProducesResponseType(typeof(BaseResponse<GetGasConsumptionsDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGasConsumptionsDictionary()
    {
        return Ok(await mediator.Send(new GetGasConsumptionsDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca zużycia gazu
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("consumptions")]
    [HasPermission(AppPermissions.Gas.ConsumptionsView)]
    [ProducesResponseType(typeof(BaseResponse<GetGasConsumptionsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGasConsumptions([FromQuery] GetGasConsumptionsQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetGasConsumptionsQuery(filters)));
    }

    /// <summary>
    /// Zwraca koszt użytego gazu
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("consumptions/calculate-cost")]
    [HasPermission(AppPermissions.Gas.ConsumptionsView)]
    [ProducesResponseType(typeof(BaseResponse<CalculateCostForConsumptionQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CalculateCostForConsumption([FromQuery] CalculateCostForConsumptionQuery query)
    {
        return Ok(await mediator.Send(query));
    }

    /// <summary>
    /// Dodaje nowe zużycie gazu
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("consumptions/add")]
    [HasPermission(AppPermissions.Gas.ConsumptionsView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddGasConsumption(AddGasConsumptionCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// "Usuwa" zużycie gazu
    /// </summary>
    /// <param name="gasConsumptionId"></param>
    /// <returns></returns>
    [HttpDelete("consumptions/delete/{gasConsumptionId:guid}")]
    [HasPermission(AppPermissions.Gas.ConsumptionsView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteGasConsumption([FromRoute] Guid gasConsumptionId)
    {
        return Ok(await mediator.Send(new DeleteGasConsumptionCommand(gasConsumptionId)));
    }

    /// <summary>
    /// Aktualizuje zużycie gazu
    /// </summary>
    /// <param name="gasConsumptionId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("consumptions/update/{gasConsumptionId:guid}")]
    [HasPermission(AppPermissions.Gas.ConsumptionsView)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateGasConsumption([FromRoute] Guid gasConsumptionId,
        UpdateGasConsumptionDto data)
    {
        return Ok(await mediator.Send(new UpdateGasConsumptionCommand(gasConsumptionId, data)));
    }
}