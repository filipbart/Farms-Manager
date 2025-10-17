using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.ExpenseAdvancePermissions;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Application.Queries.ExpenseAdvancePermissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class ExpenseAdvancePermissionsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca listę wszystkich aktywnych ewidencji zaliczek
    /// </summary>
    /// <returns></returns>
    [HttpGet("registries")]
    [ProducesResponseType(typeof(BaseResponse<GetExpenseAdvanceRegistriesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRegistries()
    {
        return Ok(await mediator.Send(new GetExpenseAdvanceRegistriesQuery()));
    }

    /// <summary>
    /// Zwraca wszystkie uprawnienia użytkownika do ewidencji zaliczek
    /// </summary>
    /// <param name="userId">ID użytkownika</param>
    /// <returns></returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(BaseResponse<UserExpenseAdvancePermissionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserPermissions([FromRoute] Guid userId)
    {
        return Ok(await mediator.Send(new GetUserExpenseAdvancePermissionsQuery(userId)));
    }

    /// <summary>
    /// Przypisuje uprawnienia do ewidencji dla użytkownika
    /// </summary>
    /// <param name="data">Dane przypisania uprawnień</param>
    /// <returns></returns>
    [HttpPost("assign")]
    [ProducesResponseType(typeof(BaseResponse<List<ExpenseAdvancePermissionDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AssignPermissions([FromBody] AssignExpenseAdvancePermissionsData data)
    {
        var result = await mediator.Send(new AssignExpenseAdvancePermissionsCommand(data));
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Aktualizuje uprawnienia użytkownika do ewidencji
    /// </summary>
    /// <param name="data">Dane aktualizacji uprawnień</param>
    /// <returns></returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(BaseResponse<List<ExpenseAdvancePermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdatePermissions([FromBody] UpdateExpenseAdvancePermissionsData data)
    {
        return Ok(await mediator.Send(new UpdateExpenseAdvancePermissionsCommand(data)));
    }

    /// <summary>
    /// Usuwa uprawnienie użytkownika do ewidencji
    /// </summary>
    /// <param name="permissionId">ID uprawnienia do usunięcia</param>
    /// <returns></returns>
    [HttpDelete("{permissionId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeletePermission([FromRoute] Guid permissionId)
    {
        await mediator.Send(new DeleteExpenseAdvancePermissionCommand(permissionId));
        return NoContent();
    }
}
