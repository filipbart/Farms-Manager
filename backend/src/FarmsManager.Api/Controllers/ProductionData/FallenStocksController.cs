using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.FallenStock;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/fallen-stocks")]
public class FallenStocksController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca dane dla sztuk upadłych
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetFallenStocksQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFallenStocks([FromQuery] GetFallenStocksQuery query)
    {
        return Ok(await mediator.Send(query));
    }

    /// <summary>
    /// Zwraca słownik filtrów
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetFallenStocksDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFallenStocksDictionary()
    {
        return Ok(await mediator.Send(new GetFallenStocksDictionaryQuery()));
    }
}