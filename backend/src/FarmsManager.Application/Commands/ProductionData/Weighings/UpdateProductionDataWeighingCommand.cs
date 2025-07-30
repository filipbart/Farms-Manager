using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.ProductionData;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.Weighings;

public record UpdateProductionDataWeighingCommandDto
{
    public int WeighingNumber { get; init; }
    public int Day { get; init; }
    public decimal Weight { get; init; }
}

public record UpdateProductionDataWeighingCommand(Guid Id, UpdateProductionDataWeighingCommandDto Data)
    : IRequest<EmptyBaseResponse>;

public class
    UpdateProductionDataWeighingCommandHandler : IRequestHandler<UpdateProductionDataWeighingCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataWeighingRepository _weighingRepository;

    public UpdateProductionDataWeighingCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataWeighingRepository weighingRepository)
    {
        _userDataResolver = userDataResolver;
        _weighingRepository = weighingRepository;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateProductionDataWeighingCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var weighing =
            await _weighingRepository.GetAsync(new ProductionDataWeighingByIdSpec(request.Id), cancellationToken);

        weighing.UpdateWeighing(request.Data.WeighingNumber, request.Data.Day, request.Data.Weight);
        weighing.SetModified(userId);
        await _weighingRepository.UpdateAsync(weighing, cancellationToken);

        return new EmptyBaseResponse();
    }
}

public class UpdateProductionDataWeighingValidator : AbstractValidator<UpdateProductionDataWeighingCommand>
{
    public UpdateProductionDataWeighingValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotNull();

        RuleFor(x => x.Data.WeighingNumber).InclusiveBetween(1, 5).WithMessage("Numer ważenia musi być od 1 do 5.");
        RuleFor(x => x.Data.Day).GreaterThanOrEqualTo(0).WithMessage("Doba nie może być ujemna.");
        RuleFor(x => x.Data.Weight).GreaterThan(0).WithMessage("Masa ciała musi być większa od 0.");
    }
}