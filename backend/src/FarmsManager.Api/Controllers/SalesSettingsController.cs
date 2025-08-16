using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.SalesSettings;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.SalesSettings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[Route("api/sales-settings")]
[HasPermission(AppPermissions.Sales.SettingsManage)]
public class SalesSettingsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca nazwy pól dodatkowych dla sprzedaży
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetSaleFieldsExtraQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSaleFieldsExtra()
    {
        return Ok(await mediator.Send(new GetSaleFieldsExtraQuery()));
    }

    /// <summary>
    /// Dodaje pole dodatkowe dla sprzedaży
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddSaleFieldExtra(AddSaleFieldExtraCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa pole dodatkowe dla sprzedaży
    /// </summary>
    /// <param name="saleFieldExtraId"></param>
    /// <returns></returns>
    [HttpDelete("delete/{saleFieldExtraId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteSaleFieldExtra([FromRoute] Guid saleFieldExtraId)
    {
        return Ok(await mediator.Send(new DeleteSaleFieldExtraCommand(saleFieldExtraId)));
    }
}