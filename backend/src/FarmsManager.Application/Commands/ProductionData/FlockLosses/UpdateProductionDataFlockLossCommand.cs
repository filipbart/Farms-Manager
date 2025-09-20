using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.FlockLosses;

public record UpdateProductionDataFlockLossCommandDto
{
    public Guid CycleId { get; init; }
    public int MeasureNumber { get; init; }
    public int Day { get; init; }
    public int Quantity { get; init; }
}

public record UpdateProductionDataFlockLossCommand(Guid Id, UpdateProductionDataFlockLossCommandDto Data)
    : IRequest<EmptyBaseResponse>;

public class
    UpdateProductionDataFlockLossCommandHandler : IRequestHandler<UpdateProductionDataFlockLossCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataFlockLossMeasureRepository _flockLossRepository;
    private readonly ICycleRepository _cycleRepository;

    public UpdateProductionDataFlockLossCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataFlockLossMeasureRepository flockLossRepository, ICycleRepository cycleRepository)
    {
        _userDataResolver = userDataResolver;
        _flockLossRepository = flockLossRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateProductionDataFlockLossCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var flockLossMeasure =
            await _flockLossRepository.GetAsync(new ProductionDataFlockLossMeasureByIdSpec(request.Id),
                cancellationToken);

        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), cancellationToken);

        if (flockLossMeasure.CycleId != cycle.Id)
        {
            flockLossMeasure.SetCycle(cycle.Id);
        }


        flockLossMeasure.UpdateMeasure(request.Data.MeasureNumber, request.Data.Day, request.Data.Quantity);
        flockLossMeasure.SetModified(userId);
        await _flockLossRepository.UpdateAsync(flockLossMeasure, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public class UpdateProductionDataFlockLossValidator : AbstractValidator<UpdateProductionDataFlockLossCommand>
{
    public UpdateProductionDataFlockLossValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotNull();

        RuleFor(x => x.Data.MeasureNumber).InclusiveBetween(1, 4).WithMessage("Numer pomiaru musi być od 1 do 4.");
        RuleFor(x => x.Data.Day).GreaterThanOrEqualTo(0).WithMessage("Doba nie może być ujemna.");
        RuleFor(x => x.Data.Quantity).GreaterThan(0).WithMessage("Ilość musi być większa od 0.");
    }
}

public sealed class ProductionDataFlockLossMeasureByIdSpec : BaseSpecification<ProductionDataFlockLossMeasureEntity>,
    ISingleResultSpecification<ProductionDataFlockLossMeasureEntity>
{
    public ProductionDataFlockLossMeasureByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(e => e.Id == id);
    }
}