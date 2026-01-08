using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.TaxBusinessEntities;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.TaxBusinessEntities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[Microsoft.AspNetCore.Mvc.Route("api/tax-business-entities")]
public class TaxBusinessEntitiesController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Pobiera listę wszystkich podmiotów gospodarczych
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetAllTaxBusinessEntitiesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await mediator.Send(new GetAllTaxBusinessEntitiesQuery()));
    }

    /// <summary>
    /// Dodaje nowy podmiot gospodarczy
    /// </summary>
    [HttpPost("add")]
    [HasPermission(AppPermissions.Data.TaxBusinessEntitiesManage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Add(AddTaxBusinessEntityCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Aktualizuje dane podmiotu gospodarczego
    /// </summary>
    [HttpPatch("update/{id:guid}")]
    [HasPermission(AppPermissions.Data.TaxBusinessEntitiesManage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromRoute] Guid id, UpdateTaxBusinessEntityDto data)
    {
        return Ok(await mediator.Send(new UpdateTaxBusinessEntityCommand(id, data)));
    }

    /// <summary>
    /// Usuwa podmiot gospodarczy
    /// </summary>
    [HttpPost("delete/{id:guid}")]
    [HasPermission(AppPermissions.Data.TaxBusinessEntitiesManage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteTaxBusinessEntityCommand(id)));
    }
}
