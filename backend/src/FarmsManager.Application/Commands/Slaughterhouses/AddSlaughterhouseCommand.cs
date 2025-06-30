using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Entities;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Slaughterhouses;

public class AddSlaughterhouseCommand : IRequest<EmptyBaseResponse>
{
    public string Name { get; init; }
    public string ProdNumber { get; init; }
    public string FullName { get; init; }
    public string Nip { get; init; }
    public string Address { get; init; }
}

public class AddSlaughterhouseCommandValidator : AbstractValidator<AddSlaughterhouseCommand>
{
    public AddSlaughterhouseCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.Nip).NotEmpty();
    }
}

public class AddSlaughterhouseCommandHandler : IRequestHandler<AddSlaughterhouseCommand, EmptyBaseResponse>
{
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddSlaughterhouseCommandHandler(ISlaughterhouseRepository slaughterhouseRepository,
        IUserDataResolver userDataResolver)
    {
        _slaughterhouseRepository = slaughterhouseRepository;
        _userDataResolver = userDataResolver;
    }


    public async Task<EmptyBaseResponse> Handle(AddSlaughterhouseCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var newHatcher = SlaughterhouseEntity.CreateNew(request.Name, request.ProdNumber, request.Nip,
            request.Address, userId);
        await _slaughterhouseRepository.AddAsync(newHatcher, cancellationToken);

        return new EmptyBaseResponse();
    }
}