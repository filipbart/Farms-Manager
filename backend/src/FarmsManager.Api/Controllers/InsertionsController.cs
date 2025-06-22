using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Insertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class InsertionsController(IMediator mediator) : BaseController
{
}