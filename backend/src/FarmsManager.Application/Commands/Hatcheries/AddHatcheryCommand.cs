using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public class AddHatcheryCommand : IRequest<EmptyBaseResponse>
{
    public string Name { get; init; }
    public string ProdNumber { get; init; }
    public string FullName { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

public class AddHatcheryCommandValidator : AbstractValidator<AddHatcheryCommand>
{
    public AddHatcheryCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.FullName).NotEmpty();
        RuleFor(t => t.Nip).NotEmpty();
    }
}

public class AddHatcheryCommandHandler : IRequestHandler<AddHatcheryCommand, EmptyBaseResponse>
{
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddHatcheryCommandHandler(IHatcheryRepository hatcheryRepository, IUserDataResolver userDataResolver)
    {
        _hatcheryRepository = hatcheryRepository;
        _userDataResolver = userDataResolver;
    }


    public async Task<EmptyBaseResponse> Handle(AddHatcheryCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var newHatcher = HatcheryEntity.CreateNew(request.Name, request.ProdNumber, request.FullName, request.Nip,
            request.Address, userId);
        await _hatcheryRepository.AddAsync(newHatcher, cancellationToken);

        return new EmptyBaseResponse();
    }
}