using FarmsManager.Api.Attributes;
using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Users;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Permissions;
using FarmsManager.Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class UsersController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca listę użytkowników
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet]
    [HasPermission(AppPermissions.Settings.Users.View)]
    [ProducesResponseType(typeof(BaseResponse<GetUsersQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetUsersQuery(filters)));
    }

    /// <summary>
    /// Dodaje nowego użytkownika
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [HasPermission(AppPermissions.Settings.Users.Manage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddUser(AddUserData data)
    {
        return Ok(await mediator.Send(new AddUserCommand(data)));
    }

    /// <summary>
    /// Usuwa użytkownika
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete("{userId:guid}/delete")]
    [HasPermission(AppPermissions.Settings.Users.Manage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
    {
        return Ok(await mediator.Send(new DeleteUserCommand(userId)));
    }

    /// <summary>
    /// Zwraca szczegóły użytkownika
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId:guid}")]
    [HasPermission(AppPermissions.Settings.Users.Manage)]
    [ProducesResponseType(typeof(BaseResponse<UserDetailsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserDetails([FromRoute] Guid userId)
    {
        return Ok(await mediator.Send(new GetUserDetailsQuery(userId)));
    }

    /// <summary>
    /// Aktualizuje użytkownika
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("{userId:guid}/update")]
    [HasPermission(AppPermissions.Settings.Users.Manage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid userId, UpdateUserData data)
    {
        return Ok(await mediator.Send(new UpdateUserCommand(userId, data)));
    }

    /// <summary>
    /// Zwraca listę wszystkich uprawnień 
    /// </summary>
    /// <returns></returns>
    [HttpGet("permissions")]
    [HasPermission(AppPermissions.Settings.Users.Manage)]
    [ProducesResponseType(typeof(BaseResponse<GetPermissionsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllPermissions()
    {
        return Ok(await mediator.Send(new GetPermissionsQuery()));
    }

    /// <summary>
    /// Aktualizuje uprawnienia uzytkownika
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost("{userId:guid}/update-permissions")]
    [HasPermission(AppPermissions.Settings.Users.Manage)]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUserPermissions([FromRoute] Guid userId,
        [FromBody] UpdateUserPermissionsDto data)
    {
        return Ok(await mediator.Send(new UpdateUserPermissionsCommand(userId, data.Permissions)));
    }
}