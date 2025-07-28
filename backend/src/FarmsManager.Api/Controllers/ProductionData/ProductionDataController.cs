using FarmsManager.Api.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/production-data")]
public class ProductionDataController(IMediator mediator) : BaseController
{
}