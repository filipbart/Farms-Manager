using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Models;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Sales;

public record UpdateSaleCommandDto
{
    /// <summary>
    /// Data sprzedaży
    /// </summary>
    public DateOnly SaleDate { get; init; }

    /// <summary>
    /// Masa całkowita sprzedanych sztuk (w kg)
    /// </summary>
    public decimal Weight { get; init; }

    /// <summary>
    /// Ilość sprzedanych sztuk
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Masa sztuk skonfiskowanych (w kg)
    /// </summary>
    public decimal ConfiscatedWeight { get; init; }

    /// <summary>
    /// Liczba sztuk skonfiskowanych
    /// </summary>
    public int ConfiscatedCount { get; init; }

    /// <summary>
    /// Masa sztuk martwych (w kg)
    /// </summary>
    public decimal DeadWeight { get; init; }

    /// <summary>
    /// Liczba sztuk martwych
    /// </summary>
    public int DeadCount { get; init; }

    /// <summary>
    /// Masa wskazana przez hodowcę (w kg)
    /// </summary>
    public decimal FarmerWeight { get; init; }

    /// <summary>
    /// Cena bazowa za 1 kg (zł)
    /// </summary>
    public decimal BasePrice { get; init; }

    /// <summary>
    /// Cena końcowa z uwzględnieniem dodatków (zł)
    /// </summary>
    public decimal PriceWithExtras { get; init; }

    /// <summary>
    /// Komentarz
    /// </summary>
    public string Comment { get; init; }

    /// <summary>
    /// Lista dodatkowych opłat lub bonusów doliczonych do ceny
    /// </summary>
    public IEnumerable<SaleOtherExtras> OtherExtras { get; init; }
}

public record UpdateSaleCommand(Guid Id, UpdateSaleCommandDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleRepository _saleRepository;

    public UpdateSaleCommandHandler(IUserDataResolver userDataResolver, ISaleRepository saleRepository)
    {
        _userDataResolver = userDataResolver;
        _saleRepository = saleRepository;
    }


    public async Task<EmptyBaseResponse> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var sale = await _saleRepository.GetAsync(new SaleByIdSpec(request.Id), cancellationToken);

        sale.Update(
            request.Data.SaleDate,
            request.Data.Weight,
            request.Data.Quantity,
            request.Data.ConfiscatedWeight,
            request.Data.ConfiscatedCount,
            request.Data.DeadWeight,
            request.Data.DeadCount,
            request.Data.FarmerWeight,
            request.Data.BasePrice,
            request.Data.PriceWithExtras,
            request.Data.Comment,
            request.Data.OtherExtras
        );

        sale.SetModified(userId);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return new EmptyBaseResponse();
    }

    private static bool AreExtrasEqual(List<SaleOtherExtras> a, List<SaleOtherExtras> b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;

        var orderedA = a.OrderBy(x => x.Name).ToList();
        var orderedB = b.OrderBy(x => x.Name).ToList();

        return !orderedA.Where((t, i) => t.Name != orderedB[i].Name || t.Value != orderedB[i].Value).Any();
    }
}