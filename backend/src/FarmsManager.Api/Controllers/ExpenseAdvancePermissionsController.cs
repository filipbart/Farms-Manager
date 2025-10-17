using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.ExpenseAdvancePermissions;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ExpenseAdvancePermissions;
using FarmsManager.Application.Queries.ExpenseAdvancePermissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

[Route("api/expenses/advances")]
public class ExpenseAdvancePermissionsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca listę wszystkich pracowników z ewidencjami zaliczek
    /// </summary>
    /// <returns></returns>
    [HttpGet("list")]
    [ProducesResponseType(typeof(BaseResponse<GetExpenseAdvancesListQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpenseAdvancesList()
    {
        return Ok(await mediator.Send(new GetExpenseAdvancesListQuery()));
    }

    /// <summary>
    /// Zwraca wszystkie uprawnienia użytkownika do ewidencji zaliczek
    /// </summary>
    /// <param name="userId">ID użytkownika</param>
    /// <returns></returns>
    [HttpGet("permissions/user/{userId:guid}")]
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
    [HttpPost("permissions/assign")]
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
    [HttpPut("permissions/update")]
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
    [HttpDelete("permissions/{permissionId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeletePermission([FromRoute] Guid permissionId)
    {
        await mediator.Send(new DeleteExpenseAdvancePermissionCommand(permissionId));
        return NoContent();
    }
}
