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
    public Guid HatcheryId { get; init; }
    public decimal Price { get; init; }
    public DateOnly Date { get; init; }
    public string Comment { get; init; }
}

public class AddHatcheryPriceCommandValidator : AbstractValidator<AddHatcheryPriceCommand>
{
    public AddHatcheryPriceCommandValidator()
    {
        RuleFor(x => x.HatcheryId)
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
    private readonly IHatcheryRepository _hatcheryRepository;
    private readonly IUserDataResolver _userDataResolver;

    public AddHatcheryPriceCommandHandler(IHatcheryPriceRepository hatcheryPriceRepository,
        IHatcheryRepository hatcheryRepository, IUserDataResolver userDataResolver)
    {
        _hatcheryPriceRepository = hatcheryPriceRepository;
        _hatcheryRepository = hatcheryRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(AddHatcheryPriceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var hatchery = await _hatcheryRepository.GetAsync(new HatcheryByIdSpec(request.HatcheryId), cancellationToken);

        var existingPrice = await _hatcheryPriceRepository.FirstOrDefaultAsync(
            new HatcheryPriceByHatcheryAndDateSpec(request.HatcheryId, request.Date),
            cancellationToken);

        if (existingPrice is not null)
        {
            throw new Exception("Cena dla tej wylęgarni w podanym dniu już istnieje.");
        }

        var newPrice = HatcheryPriceEntity.CreateNew(
            hatchery.Id,
            request.Price,
            request.Date,
            request.Comment,
            userId
        );

        await _hatcheryPriceRepository.AddAsync(newPrice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public sealed class HatcheryPriceByHatcheryAndDateSpec : BaseSpecification<HatcheryPriceEntity>,
    ISingleResultSpecification<HatcheryPriceEntity>
{
    public HatcheryPriceByHatcheryAndDateSpec(Guid id, DateOnly date)
    {
        EnsureExists();
        Query.Where(x => x.HatcheryId == id);
        Query.Where(x => x.Date == date);
    }
}