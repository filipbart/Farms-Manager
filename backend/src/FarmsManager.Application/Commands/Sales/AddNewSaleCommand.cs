using FarmsManager.Application.Commands.Farms;
using FarmsManager.Application.Commands.Slaughterhouses;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Services;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Application.Specifications.Farms;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Enums;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.FarmAggregate.Models;
using FarmsManager.Domain.Aggregates.SlaughterhouseAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Sales;

public record AddNewSaleCommandDto
{
    public SaleType SaleType { get; init; }
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly SaleDate { get; init; }
    public Guid SlaughterhouseId { get; init; }
    public string Entries { get; init; }
    public List<IFormFile> Files { get; init; }
}

public record AddNewSaleCommand : IRequest<BaseResponse<AddNewSaleCommandResponse>>
{
    public SaleType SaleType { get; init; }
    public Guid FarmId { get; init; }
    public Guid CycleId { get; init; }
    public DateOnly SaleDate { get; init; }
    public Guid SlaughterhouseId { get; init; }
    public List<Entry> Entries { get; init; }
    public List<IFormFile> Files { get; init; }

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
        public IEnumerable<OtherExtra> OtherExtras { get; init; }
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
    public AddNewSaleCommandHandler(IUserDataResolver userDataResolver, ISaleRepository saleRepository,
        IFarmRepository farmRepository, ICycleRepository cycleRepository, IHenhouseRepository henhouseRepository,
        ISlaughterhouseRepository slaughterhouseRepository, IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _saleRepository = saleRepository;
        _farmRepository = farmRepository;
        _cycleRepository = cycleRepository;
        _henhouseRepository = henhouseRepository;
        _slaughterhouseRepository = slaughterhouseRepository;
        _s3Service = s3Service;
    }

    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleRepository _saleRepository;
    private readonly IFarmRepository _farmRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IHenhouseRepository _henhouseRepository;
    private readonly ISlaughterhouseRepository _slaughterhouseRepository;
    private readonly IS3Service _s3Service;

    public async Task<BaseResponse<AddNewSaleCommandResponse>> Handle(AddNewSaleCommand request,
        CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var farm = await _farmRepository.GetAsync(new FarmByIdSpec(request.FarmId), ct);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.CycleId), ct);
        var slaughterhouse =
            await _slaughterhouseRepository.GetAsync(new SlaughterhouseByIdSpec(request.SlaughterhouseId), ct);

        var newSales = new List<SaleEntity>();
        var internalGroupId = Guid.NewGuid();

        var directoryGuid = Guid.NewGuid();
        FileDirectoryModel fileDirectoryModel = null;

        if (request.Files != null && request.Files.Count != 0)
        {
            foreach (var file in request.Files)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream, ct);
                var fileBytes = memoryStream.ToArray();

                fileDirectoryModel = await _s3Service.UploadFileToDirectoryAsync(fileBytes, FileType.Sales,
                    directoryGuid.ToString(), file.FileName);
            }
        }

        foreach (var entry in request.Entries)
        {
            var henhouse = await _henhouseRepository.GetAsync(new HenhouseByIdSpec(entry.HenhouseId),
                ct);

            var otherExtras = entry.OtherExtras?.Select(t => new SaleOtherExtras
            {
                Name = t.Name,
                Value = t.Value
            });

            var newSale = SaleEntity.CreateNew(internalGroupId, request.SaleType, request.SaleDate, farm.Id, cycle.Id,
                slaughterhouse.Id, henhouse.Id, entry.Weight, entry.Quantity, entry.ConfiscatedWeight,
                entry.ConfiscatedCount, entry.DeadWeight, entry.DeadCount, entry.FarmerWeight, entry.BasePrice,
                entry.PriceWithExtras, entry.Comment, otherExtras, userId);

            newSale.SetDirectoryPath(fileDirectoryModel?.DirectoryPath);

            newSales.Add(newSale);
        }

        if (newSales.Count != 0)
            await _saleRepository.AddRangeAsync(newSales, ct);


        return BaseResponse.CreateResponse(new AddNewSaleCommandResponse
        {
            InternalGroupId = internalGroupId
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