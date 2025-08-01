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

public record UpdateHatcheryPriceData
{
    public decimal Price { get; init; }
    public DateOnly Date { get; init; }
    public string Comment { get; init; }
}

public record UpdateHatcheryPriceCommand(Guid Id, UpdateHatcheryPriceData Data) : IRequest<EmptyBaseResponse>;

public class UpdateHatcheryPriceCommandValidator : AbstractValidator<UpdateHatcheryPriceCommand>
{
    public UpdateHatcheryPriceCommandValidator()
    {
        RuleFor(x => x.Data.Price)
            .GreaterThan(0).WithMessage("Cena musi być większa od zera.");

        RuleFor(x => x.Data.Date)
            .NotEmpty().WithMessage("Data jest wymagana.");
    }
}

public class UpdateHatcheryPriceCommandHandler : IRequestHandler<UpdateHatcheryPriceCommand, EmptyBaseResponse>
{
    private readonly IHatcheryPriceRepository _hatcheryPriceRepository;
    private readonly IUserDataResolver _userDataResolver;

    public UpdateHatcheryPriceCommandHandler(IHatcheryPriceRepository hatcheryPriceRepository,
        IUserDataResolver userDataResolver)
    {
        _hatcheryPriceRepository = hatcheryPriceRepository;
        _userDataResolver = userDataResolver;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateHatcheryPriceCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var hatcheryPrice =
            await _hatcheryPriceRepository.GetAsync(new HatcheryPriceByIdSpec(request.Id), cancellationToken);

        if (hatcheryPrice.Date != request.Data.Date)
        {
            var existingPriceOnNewDate = await _hatcheryPriceRepository.FirstOrDefaultAsync(
                new HatcheryPriceByHatcheryAndDateSpec(hatcheryPrice.HatcheryId, request.Data.Date),
                cancellationToken);


            if (existingPriceOnNewDate is not null && existingPriceOnNewDate.Id != hatcheryPrice.Id)
            {
                throw new Exception("Cena dla tej wylęgarni w podanym dniu już istnieje.");
            }
        }

        hatcheryPrice.Update(
            request.Data.Price,
            request.Data.Date,
            request.Data.Comment
        );
        hatcheryPrice.SetModified(userId);

        await _hatcheryPriceRepository.UpdateAsync(hatcheryPrice, cancellationToken);

        return BaseResponse.EmptyResponse;
    }
}

public sealed class HatcheryPriceByIdSpec : BaseSpecification<HatcheryPriceEntity>,
    ISingleResultSpecification<HatcheryPriceEntity>
{
    public HatcheryPriceByIdSpec(Guid id)
    {
        EnsureExists();
        Query.Where(t => t.Id == id);
    }
}