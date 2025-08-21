using Ardalis.Specification;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.FarmAggregate.Entities;
using FarmsManager.Domain.Aggregates.FarmAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;
using FluentValidation;

namespace FarmsManager.Application.Commands.Insertions;

public record MarkInsertionAsReportedToWiosCommand(List<Guid> InsertionIds, string Comment)
    : IRequest<EmptyBaseResponse>;

public class MarkInsertionAsReportedToWiosCommandValidator : AbstractValidator<MarkInsertionAsReportedToWiosCommand>
{
    public MarkInsertionAsReportedToWiosCommandValidator()
    {
        RuleFor(x => x.InsertionIds)
            .NotEmpty()
            .WithMessage("Należy wybrać co najmniej jedno wstawienie.");
    }
}

public class
    MarkInsertionAsReportedToWiosCommandHandler : IRequestHandler<MarkInsertionAsReportedToWiosCommand,
    EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IInsertionRepository _insertionRepository;

    public MarkInsertionAsReportedToWiosCommandHandler(IUserDataResolver userDataResolver,
        IInsertionRepository insertionRepository)
    {
        _userDataResolver = userDataResolver;
        _insertionRepository = insertionRepository;
    }

    public async Task<EmptyBaseResponse> Handle(MarkInsertionAsReportedToWiosCommand request, CancellationToken ct)
    {
        var response = new EmptyBaseResponse();
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var insertions = await _insertionRepository.ListAsync(new GetInsertionsByIdsSpec(request.InsertionIds), ct);

        if (insertions.Count != request.InsertionIds.Count)
        {
            response.AddError("Insertions", "Nie znaleziono wszystkich wstawień o podanych identyfikatorach.");
            return response;
        }

        if (insertions.Any(i => i.ReportedToWios))
        {
            response.AddError("Insertions", "Co najmniej jedno z wybranych wstawień zostało już zgłoszone do WIOŚ.");
            return response;
        }

        foreach (var insertion in insertions)
        {
            insertion.MarkAsReportedToWios(request.Comment);
            insertion.SetModified(userId);
        }

        await _insertionRepository.UpdateRangeAsync(insertions, ct);

        return response;
    }
}

public sealed class GetInsertionsByIdsSpec : BaseSpecification<InsertionEntity>
{
    public GetInsertionsByIdsSpec(List<Guid> ids)
    {
        EnsureExists();
        Query.Where(i => ids.Contains(i.Id));
    }
}