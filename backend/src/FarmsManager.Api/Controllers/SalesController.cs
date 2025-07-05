using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.Sales;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Sales;
using FarmsManager.Application.Queries.Slaughterhouses.Dictionary;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class SalesController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca słowniki filtrów
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetSalesDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionaries()
    {
        return Ok(await mediator.Send(new GetSalesDictionaryQuery()));
    }
    
    /// <summary>
    /// Zwraca sprzedaże według podanych filtrów
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetSalesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSales([FromQuery] GetSalesQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetSalesQuery(filters)));
    }
    
    /// <summary>
    /// Dodaje nową sprzedaz
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(BaseResponse<AddNewSaleCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNewInsertion(AddNewSaleCommand command)
    {
        return Ok(await mediator.Send(command));
    }
}