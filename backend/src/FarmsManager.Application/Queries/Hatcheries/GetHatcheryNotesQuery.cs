using Ardalis.Specification;
using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Extensions;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Entities;
using FarmsManager.Domain.Aggregates.HatcheryAggregate.Interfaces;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.Hatcheries;

public class HatcheryNoteDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Content { get; init; }
    public int Order { get; init; }
    public DateTime DateCreatedUtc { get; init; }
    public string CreatedByName { get; init; }
    public DateTime? DateModifiedUtc { get; init; }
    public string ModifiedByName { get; init; }
    public DateTime? DateDeletedUtc { get; init; }
    public string DeletedByName { get; init; }
}

public record GetHatcheryNotesQueryResponse
{
    public List<HatcheryNoteDto> Items { get; init; }
}

public record GetHatcheryNotesQuery(bool? ShowDeleted = null) : IRequest<BaseResponse<GetHatcheryNotesQueryResponse>>;

public sealed class GetAllHatcheryNotesSpec : BaseSpecification<HatcheryNoteEntity>
{
    public GetAllHatcheryNotesSpec(bool? showDeleted, bool isAdmin)
    {
        EnsureExists(showDeleted, isAdmin);
        Query.OrderBy(x => x.DateCreatedUtc);
    }
}

public class GetHatcheryNotesQueryHandler : IRequestHandler<GetHatcheryNotesQuery,
    BaseResponse<GetHatcheryNotesQueryResponse>>
{
    private readonly IHatcheryNoteRepository _hatcheryNoteRepository;
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;

    public GetHatcheryNotesQueryHandler(IHatcheryNoteRepository hatcheryNoteRepository,
        IUserDataResolver userDataResolver, IUserRepository userRepository)
    {
        _hatcheryNoteRepository = hatcheryNoteRepository;
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GetHatcheryNotesQueryResponse>> Handle(GetHatcheryNotesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        var user = await _userRepository.GetAsync(new UserByIdSpec(userId), cancellationToken);
        var isAdmin = user.IsAdmin;

        var notes = await _hatcheryNoteRepository.ListAsync<HatcheryNoteDto>(
            new GetAllHatcheryNotesSpec(request.ShowDeleted, isAdmin), cancellationToken);

        return BaseResponse.CreateResponse(new GetHatcheryNotesQueryResponse
        {
            Items = notes.ClearAdminData(isAdmin)
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