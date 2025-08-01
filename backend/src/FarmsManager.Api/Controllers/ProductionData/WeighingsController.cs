using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.ProductionData.Weighings;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Models.ProductionData;
using FarmsManager.Application.Queries.ProductionData.Weighings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FarmsManager.Api.Controllers.ProductionData;

[Route("api/production-data/weighings")]
public class WeighingsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Zwraca listę ważeń według podanych filtrów
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<GetProductionDataWeighingsQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetWeighings([FromQuery] GetProductionDataWeighingsQueryFilters filters)
    {
        return Ok(await mediator.Send(new GetProductionDataWeighingsQuery(filters)));
    }
    
    /// <summary>
    /// Zwraca słownik filtrów dla Upadki i wybrakowania
    /// </summary>
    /// <returns></returns>
    [HttpGet("dictionary")]
    [ProducesResponseType(typeof(BaseResponse<GetProductionDataWeighingsDictionaryQueryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProductionDataDictionary()
    {
        return Ok(await mediator.Send(new GetProductionDataWeighingsDictionaryQuery()));
    }

    /// <summary>
    /// Zwraca wylęgarnie przy dodawaniu ważenia
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("get-hatchery")]
    [ProducesResponseType(typeof(BaseResponse<GetHatcheryForWeighingQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetHatcheryForWeighing([FromQuery] GetHatcheryForWeighingQuery query)
    {
        return Ok(await mediator.Send(query));
    }

    /// <summary>
    /// Dodaje nowy ważenie
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddWeighing(AddProductionDataWeighingCommand command)
    {
        return Ok(await mediator.Send(command));
    }


    /// <summary>
    /// Aktualizuje dane wpisu
    /// </summary>
    [HttpPatch("update/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateWeighing([FromRoute] Guid id,
        [FromBody] UpdateProductionDataWeighingCommandDto payload)
    {
        return Ok(await mediator.Send(new UpdateProductionDataWeighingCommand(id, payload)));
    }

    /// <summary>
    /// Usuwa wpis
    /// </summary>
    [HttpDelete("delete/{id:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteWeighing([FromRoute] Guid id)
    {
        return Ok(await mediator.Send(new DeleteProductionDataWeighingCommand(id)));
    }
}