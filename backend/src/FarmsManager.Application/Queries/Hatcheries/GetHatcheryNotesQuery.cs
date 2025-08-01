using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Specifications;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using MediatR;

namespace FarmsManager.Application.Queries.Hatcheries;

public class HatcheryNoteDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Content { get; init; }
    public DateTime DateCreatedUtc { get; init; }
}

public record GetHatcheryNotesQueryResponse
{
    public List<HatcheryNoteDto> Items { get; init; }
}

public record GetHatcheryNotesQuery : IRequest<BaseResponse<GetHatcheryNotesQueryResponse>>;

public sealed class GetAllHatcheryNotesSpec : BaseSpecification<HatcheryNoteEntity>
{
    public GetAllHatcheryNotesSpec()
    {
        EnsureExists();
        Query.OrderBy(x => x.DateCreatedUtc);
    }
}

public class GetHatcheryNotesQueryHandler : IRequestHandler<GetHatcheryNotesQuery,
    BaseResponse<GetHatcheryNotesQueryResponse>>
{
    private readonly IHatcheryNoteRepository _hatcheryNoteRepository;

    public GetHatcheryNotesQueryHandler(IHatcheryNoteRepository hatcheryNoteRepository)
    {
        _hatcheryNoteRepository = hatcheryNoteRepository;
    }

    public async Task<BaseResponse<GetHatcheryNotesQueryResponse>> Handle(GetHatcheryNotesQuery request,
        CancellationToken cancellationToken)
    {
        var notes = await _hatcheryNoteRepository.ListAsync<HatcheryNoteDto>(
            new GetAllHatcheryNotesSpec(), cancellationToken);

        return BaseResponse.CreateResponse(new GetHatcheryNotesQueryResponse
        {
            Items = notes
        });
    }
}

public class HatcheryNoteProfile : Profile
{
    public HatcheryNoteProfile()
    {
        CreateMap<HatcheryNoteEntity, HatcheryNoteDto>();
    }
}