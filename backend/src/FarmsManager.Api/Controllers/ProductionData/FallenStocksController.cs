using FarmsManager.Api.Controllers.Base;
using FarmsManager.Application.Commands.FallenStocks;
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

    /// <summary>
    /// Zwraca listę dostępny kurników
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("available-henhouses")]
    [ProducesResponseType(typeof(BaseResponse<GetAvailableHenhousesQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAvailableHenhouses([FromQuery] GetAvailableHenhousesQuery query)
    {
        return Ok(await mediator.Send(query));
    }

    /// <summary>
    /// Dodaje nowe sztuki upadłe i wysyła do IRZplus
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNewFallenStocks(AddNewFallenStocksCommand command)
    {
        return Ok(await mediator.Send(command));
    }

    /// <summary>
    /// Usuwa sztuki upadłe dla wybranej grupy
    /// </summary>
    /// <param name="internalGroupId"></param>
    /// <returns></returns>
    [HttpDelete("delete/{internalGroupId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteFallenStocks([FromRoute] Guid internalGroupId)
    {
        return Ok(await mediator.Send(new DeleteFallenStocksCommand(internalGroupId)));
    }

    /// <summary>
    /// Zwraca dane do edycji dla wybranej grupy
    /// </summary>
    /// <param name="internalGroupId"></param>
    /// <returns></returns>
    [HttpGet("edit-data/{internalGroupId:guid}")]
    [ProducesResponseType(typeof(BaseResponse<GetFallenStocksDataForEditQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFallenStocksDataForEdit([FromRoute] Guid internalGroupId)
    {
        return Ok(await mediator.Send(new GetFallenStocksDataForEditQuery(internalGroupId)));
    }

    /// <summary>
    /// Aktualizuje dane sztuk padłych dla danej grupy
    /// </summary>
    /// <param name="internalGroupId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPatch("update-data/{internalGroupId:guid}")]
    [ProducesResponseType(typeof(EmptyBaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFallenStocks([FromRoute] Guid internalGroupId,
        [FromBody] UpdateFallenStocksData data)
    {
        return Ok(await mediator.Send(new UpdateFallenStocksCommand(internalGroupId, data)));
    }
}