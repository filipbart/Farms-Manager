using FarmsManager.Api.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class DevController(IMediator mediator) : BaseController
{
    [AllowAnonymous]
    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        return Ok("Działa");
    }

    [AllowAnonymous]
    [HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount()
    {
        return Ok();
    }
}