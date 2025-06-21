using FarmsManager.Application.Common.Responses;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Farms;

public record AddFarmCommand : IRequest<EmptyBaseResponse>
{
    public string Name { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

public class AddFarmCommandValidator : AbstractValidator<AddFarmCommand>
{
    public AddFarmCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.Nip).NotEmpty();
        RuleFor(t => t.Address).NotEmpty();
    }
}

public class AddCommandHandler : IRequestHandler<AddFarmCommand, EmptyBaseResponse>
{
    private readonly IFarmRepository _farmRepository;

    public AddCommandHandler(IFarmRepository farmRepository)
    {
        _farmRepository = farmRepository;
    }

    public async Task<EmptyBaseResponse> Handle(AddFarmCommand request, CancellationToken cancellationToken)
    {
        var newFarm = FarmEntity.CreateNew(request.Name, request.Nip, request.Address);
        await _farmRepository.AddAsync(newFarm, cancellationToken);

        return new EmptyBaseResponse();
    }
}