using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FluentValidation;
using MediatR;

namespace FarmsManager.Application.Commands.Sales;

public record AddNewSaleCommand : IRequest<BaseResponse<AddNewSaleCommandResponse>>
{
    public SaleType SaleType { get; init; }
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly SaleDate { get; init; }
    public Guid SlaughterhouseId { get; init; }
    public List<Entry> Entries { get; init; }

    public class Entry
    {
        public Guid HenhouseId { get; init; }
        public int Quantity { get; init; }
        public decimal Weight { get; init; }
        public int ConfiscatedCount { get; init; }
        public decimal ConfiscatedWeight { get; init; }
        public int DeadCount { get; init; }
        public decimal DeadWeight { get; init; }
        public decimal FarmerWeight { get; init; }
        public decimal BasePrice { get; init; }
        public decimal PriceWithExtras { get; init; }
        public string Comment { get; init; }
        public List<OtherExtra> OtherExtras { get; init; }
    }

    public class OtherExtra
    {
        public string Name { get; init; }
        public decimal Value { get; init; }
    }
}

public record AddNewSaleCommandResponse
{
    public Guid InternalGroupId { get; init; }
}

public class AddNewSaleCommandHandler : IRequestHandler<AddNewSaleCommand, BaseResponse<AddNewSaleCommandResponse>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleRepository _saleRepository;
    public async Task<BaseResponse<AddNewSaleCommandResponse>> Handle(AddNewSaleCommand request,
        CancellationToken ct)
    {
        
        
        return BaseResponse.CreateResponse(new AddNewSaleCommandResponse
        {
            InternalGroupId = Guid.Empty
        });
    }
}

public class AddNewSaleCommandValidator : AbstractValidator<AddNewSaleCommand>
{
    public AddNewSaleCommandValidator()
    {
        RuleFor(x => x.SaleType).NotNull();
        RuleFor(x => x.FarmId).NotEmpty();
        RuleFor(x => x.CycleId).NotEmpty();
        RuleFor(x => x.SaleDate).NotNull().LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now));
        RuleFor(x => x.SlaughterhouseId).NotEmpty();
        RuleFor(x => x.Entries).NotNull().NotEmpty();
        RuleForEach(t => t.Entries).ChildRules(t =>
        {
            t.RuleFor(x => x.Quantity).GreaterThan(0);
            t.RuleFor(x => x.Weight).GreaterThan(0);
            t.RuleFor(x => x.ConfiscatedCount).GreaterThanOrEqualTo(0);
            t.RuleFor(x => x.ConfiscatedWeight).GreaterThanOrEqualTo(0);
            t.RuleFor(x => x.DeadCount).GreaterThanOrEqualTo(0);
            t.RuleFor(x => x.DeadWeight).GreaterThanOrEqualTo(0);
            t.RuleFor(x => x.FarmerWeight).GreaterThanOrEqualTo(0);
            t.RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0);
            t.RuleFor(x => x.PriceWithExtras).GreaterThanOrEqualTo(0);
        });
    }
}