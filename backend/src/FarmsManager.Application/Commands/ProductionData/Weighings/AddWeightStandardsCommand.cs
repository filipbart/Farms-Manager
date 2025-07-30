using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Entities;
using FarmsManager.Domain.Aggregates.ProductionDataAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.ProductionData.Weighings;

public record WeightStandardDto
{
    public int Day { get; init; }
    public decimal Weight { get; init; }
}

public record AddWeightStandardsCommand(List<WeightStandardDto> Standards) : IRequest<EmptyBaseResponse>;

public class AddWeightStandardsCommandHandler : IRequestHandler<AddWeightStandardsCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IProductionDataWeightStandardRepository _repository;

    public AddWeightStandardsCommandHandler(IUserDataResolver userDataResolver,
        IProductionDataWeightStandardRepository repository)
    {
        _userDataResolver = userDataResolver;
        _repository = repository;
    }

    public async Task<EmptyBaseResponse> Handle(AddWeightStandardsCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var newStandards = request.Standards
            .Select(s => ProductionDataWeightStandardEntity.CreateNew(s.Day, s.Weight, userId))
            .ToList();

        await _repository.AddRangeAsync(newStandards, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public class AddWeightStandardsValidator : AbstractValidator<AddWeightStandardsCommand>
{
    public AddWeightStandardsValidator()
    {
        RuleFor(x => x.Standards).NotEmpty();
        RuleForEach(x => x.Standards).ChildRules(standard =>
        {
            standard.RuleFor(s => s.Day).GreaterThanOrEqualTo(0);
            standard.RuleFor(s => s.Weight).GreaterThan(0);
        });
    }
}