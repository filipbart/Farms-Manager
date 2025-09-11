using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record AddHatcheryNameCommand(string Name) : IRequest<EmptyBaseResponse>;

public class AddHatcheryNameCommandValidator : AbstractValidator<AddHatcheryNameCommand>
{
    public AddHatcheryNameCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa wylęgarni jest wymagana.")
            .MaximumLength(50).WithMessage("Nazwa wylęgarni nie może być dłuższa niż 50 znaków.");
    }
}

public class AddHatcheryNameCommandHandler : IRequestHandler<AddHatcheryNameCommand, EmptyBaseResponse>
{
    private readonly IHatcheryNameRepository _hatcheryNameRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddHatcheryNameCommandHandler(IHatcheryNameRepository hatcheryNameRepository,
        IUserDataResolver userDataResolver)
    {
        _hatcheryNameRepository = hatcheryNameRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddHatcheryNameCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var existing =
            await _hatcheryNameRepository.FirstOrDefaultAsync(new HatcheryNameByNameSpec(request.Name),
                cancellationToken);
        if (existing is not null)
        {
            throw new Exception("Wylęgarnia o tej nazwie już istnieje.");
        }

        var newHatcheryName = HatcheryNameEntity.CreateNew(request.Name, userId);

        await _hatcheryNameRepository.AddAsync(newHatcheryName, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}