using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Hatcheries;

public record AddHatcheryPriceCommand : IRequest<EmptyBaseResponse>
{
    public string HatcheryName { get; init; }
    public decimal Price { get; init; }
    public DateOnly Date { get; init; }
    public string Comment { get; init; }
}

public class AddHatcheryPriceCommandValidator : AbstractValidator<AddHatcheryPriceCommand>
{
    public AddHatcheryPriceCommandValidator()
    {
        RuleFor(x => x.HatcheryName)
            .NotEmpty().WithMessage("Wylęgarnia jest wymagana.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Cena musi być większa od zera.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Data jest wymagana.");
    }
}

public class AddHatcheryPriceCommandHandler : IRequestHandler<AddHatcheryPriceCommand, EmptyBaseResponse>
{
    private readonly IHatcheryPriceRepository _hatcheryPriceRepository;
    private readonly IHatcheryNameRepository _hatcheryNameRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddHatcheryPriceCommandHandler(IHatcheryPriceRepository hatcheryPriceRepository,
        IHatcheryNameRepository hatcheryNameRepository, IUserDataResolver userDataResolver)
    {
        _hatcheryPriceRepository = hatcheryPriceRepository;
        _hatcheryNameRepository = hatcheryNameRepository;
        _userDataResolver = userDataResolver;
    }


    public async Task<EmptyBaseResponse> Handle(AddHatcheryPriceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var hatcheryName =
            await _hatcheryNameRepository.GetAsync(new HatcheryNameByNameSpec(request.HatcheryName), cancellationToken);

        var existingPrice = await _hatcheryPriceRepository.FirstOrDefaultAsync(
            new HatcheryPriceByHatcheryNameAndDateSpec(request.HatcheryName, request.Date),
            cancellationToken);

        if (existingPrice is not null)
        {
            throw new Exception("Cena dla tej wylęgarni w podanym dniu już istnieje.");
        }

        var newPrice = HatcheryPriceEntity.CreateNew(
            hatcheryName.Name,
            request.Price,
            request.Date,
            request.Comment,
            userId
        );

        await _hatcheryPriceRepository.AddAsync(newPrice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public sealed class HatcheryPriceByHatcheryNameAndDateSpec : BaseSpecification<HatcheryPriceEntity>,
    ISingleResultSpecification<HatcheryPriceEntity>
{
    public HatcheryPriceByHatcheryNameAndDateSpec(string hatcheryName, DateOnly date)
    {
        EnsureExists();
        Query.Where(x => x.HatcheryName == hatcheryName);
        Query.Where(x => x.Date == date);
    }
}

public sealed class HatcheryNameByNameSpec : BaseSpecification<HatcheryNameEntity>,
    ISingleResultSpecification<HatcheryNameEntity>
{
    public HatcheryNameByNameSpec(string name)
    {
        EnsureExists();
        DisableTracking();
        Query.Where(x => x.Name == name);
    }
}