using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Insertions;
using FarmsManager.Application.Queries.Summary;
using FarmsManager.Application.Queries.Summary.FinancialAnalysis;
using FarmsManager.Application.Queries.Summary.ProductionAnalysis;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers;

public class SummaryController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca słowniki filtrów
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetSummaryDictionaryQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDictionaries()
    {
        return Ok(await mediator.Send(new GetSummaryDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca dane dla podsumowania - Analiza produkcyjna
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("production-analysis")]
    [ProducesResponseType(typeof(BaseResponse<SummaryProductionAnalysisQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProductionAnalysisData(
        [FromQuery] GetInsertionsQueryFilters filters)
    {
        return Ok(await mediator.Send(new SummaryProductionAnalysisQuery(filters)));
    }
    
    /// <summary>
    /// Zwraca dane dla podsumowania - Analiza finansowa
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [HttpGet("financial-analysis")]
    [ProducesResponseType(typeof(BaseResponse<SummaryFinancialAnalysisQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFinancialAnalysisData(
        [FromQuery] GetInsertionsQueryFilters filters)
    {
        return Ok(await mediator.Send(new SummaryFinancialAnalysisQuery(filters)));
    }
}