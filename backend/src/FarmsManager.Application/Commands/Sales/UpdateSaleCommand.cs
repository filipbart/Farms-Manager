using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Sales;
using FarmsManager.Domain.Aggregates.SaleAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.SaleAggregate.Models;
using FarmsManager.Domain.Exceptions;
using MediatR;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Application.Specifications.Cycle;
using FarmsManager.Shared.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Commands.Sales;

public record UpdateSaleCommandFormDto
{
    public Guid CycleId { get; init; }

    /// <summary>
    /// Data sprzedaży
    /// </summary>
    public DateOnly SaleDate { get; init; }

    /// <summary>
    /// Masa całkowita sprzedanych sztuk (w kg)
    /// </summary>
    public decimal Weight { get; init; }

    /// <summary>
    /// Ilość sprzedanych sztuk
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Masa sztuk skonfiskowanych (w kg)
    /// </summary>
    public decimal ConfiscatedWeight { get; init; }

    /// <summary>
    /// Liczba sztuk skonfiskowanych
    /// </summary>
    public int ConfiscatedCount { get; init; }

    /// <summary>
    /// Masa sztuk martwych (w kg)
    /// </summary>
    public decimal DeadWeight { get; init; }

    /// <summary>
    /// Liczba sztuk martwych
    /// </summary>
    public int DeadCount { get; init; }

    /// <summary>
    /// Masa wskazana przez hodowcę (w kg)
    /// </summary>
    public decimal FarmerWeight { get; init; }

    /// <summary>
    /// Cena bazowa za 1 kg (zł)
    /// </summary>
    public decimal BasePrice { get; init; }

    /// <summary>
    /// Cena końcowa z uwzględnieniem dodatków (zł)
    /// </summary>
    public decimal PriceWithExtras { get; init; }

    /// <summary>
    /// Komentarz
    /// </summary>
    public string Comment { get; init; }

    /// <summary>
    /// Lista dodatkowych opłat lub bonusów doliczonych do ceny (JSON string)
    /// </summary>
    public string OtherExtras { get; init; }

    /// <summary>
    /// Plik do dodania (opcjonalny)
    /// </summary>
    public IFormFile File { get; init; }
}

public record UpdateSaleCommand(Guid Id, UpdateSaleCommandFormDto Data) : IRequest<EmptyBaseResponse>;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly ISaleRepository _saleRepository;
    private readonly ICycleRepository _cycleRepository;
    private readonly IS3Service _s3Service;

    public UpdateSaleCommandHandler(IUserDataResolver userDataResolver, ISaleRepository saleRepository,
        ICycleRepository cycleRepository, IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _saleRepository = saleRepository;
        _cycleRepository = cycleRepository;
        _s3Service = s3Service;
    }

    public async Task<EmptyBaseResponse> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var sale = await _saleRepository.GetAsync(new SaleByIdSpec(request.Id), cancellationToken);
        var cycle = await _cycleRepository.GetAsync(new CycleByIdSpec(request.Data.CycleId), cancellationToken);

        var response = new EmptyBaseResponse();

        if (sale.CycleId != cycle.Id)
        {
            sale.SetCycle(cycle.Id);
        }

        // Parsowanie OtherExtras z JSON string
        IEnumerable<SaleOtherExtras> otherExtras = null;
        if (!string.IsNullOrEmpty(request.Data.OtherExtras))
        {
            try
            {
                var parsedExtras = request.Data.OtherExtras.ParseJsonString<List<SaleOtherExtras>>();
                otherExtras = parsedExtras;
            }
            catch
            {
                response.AddError("otherExtras", "Nieprawidłowy format OtherExtras");
                return response;
            }
        }

        sale.Update(
            request.Data.SaleDate,
            request.Data.Weight,
            request.Data.Quantity,
            request.Data.ConfiscatedWeight,
            request.Data.ConfiscatedCount,
            request.Data.DeadWeight,
            request.Data.DeadCount,
            request.Data.FarmerWeight,
            request.Data.BasePrice,
            request.Data.PriceWithExtras,
            request.Data.Comment,
            otherExtras
        );

        // Obsługa pliku
        if (request.Data.File != null)
        {
            var directoryGuid = Guid.NewGuid();
            using var memoryStream = new MemoryStream();
            await request.Data.File.CopyToAsync(memoryStream, cancellationToken);
            var fileBytes = memoryStream.ToArray();

            var fileDirectoryModel = await _s3Service.UploadFileToDirectoryAsync(fileBytes, FileType.Sales,
                directoryGuid.ToString(), request.Data.File.FileName);

            sale.SetDirectoryPath(fileDirectoryModel.DirectoryPath);
        }

        sale.SetModified(userId);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return response;
    }
}

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data.CycleId).NotEmpty();
        RuleFor(x => x.Data.SaleDate).NotNull().LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now));
        RuleFor(x => x.Data.Quantity).GreaterThan(0);
        RuleFor(x => x.Data.Weight).GreaterThan(0);
        RuleFor(x => x.Data.ConfiscatedCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data.ConfiscatedWeight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data.DeadCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data.DeadWeight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data.FarmerWeight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data.BasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data.PriceWithExtras).GreaterThanOrEqualTo(0);

        // Walidacja pliku
        When(x => x.Data.File != null, () =>
        {
            RuleFor(x => x.Data.File.Length)
                .LessThanOrEqualTo(50 * 1024 * 1024) // Max 50MB
                .WithMessage("Plik nie może być większy niż 50MB");

            RuleFor(x => x.Data.File.ContentType)
                .Must(IsValidFileType)
                .WithMessage("Niedozwolony typ pliku. Akceptowane: PDF, JPG, PNG, GIF");
        });
    }

    private static bool IsValidFileType(string contentType)
    {
        var allowedTypes = new[]
        {
            "application/pdf",
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp"
        };

        return allowedTypes.Contains(contentType?.ToLower());
    }
}