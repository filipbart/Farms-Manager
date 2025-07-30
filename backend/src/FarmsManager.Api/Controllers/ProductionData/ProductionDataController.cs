using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.ProductionData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/production-data")]
public class ProductionDataController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca słownik filtrów dla Upadki i wybrakowania
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetProductionDataDictionaryQueryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProductionDataDictionary()
    {
        return Ok(await mediator.Send(new GetProductionDataDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca wartość pozostałej paszy na podstawie podanych parametrów
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("calculate-value")]
    [ProducesResponseType(typeof(BaseResponse<GetRemainingFeedValueQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRemainingFeedValue([FromQuery] GetRemainingFeedValueQuery query)
    {
        return Ok(await mediator.Send(query));
    }
}