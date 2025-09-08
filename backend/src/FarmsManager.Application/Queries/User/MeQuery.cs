using AutoMapper;
using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Application.Specifications.Users;
using FarmsManager.Domain.Aggregates.UserAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Queries.User;

public record MeQuery : IRequest<BaseResponse<MeQueryResponse>>;

public record MeQueryResponse
{
    public Guid Id { get; init; }
    public string Login { get; init; }
    public string Name { get; init; }
    public bool IsAdmin { get; init; }
    public bool MustChangePassword { get; init; }
    public string AvatarPath { get; set; }
    public List<string> Permissions { get; init; }
    public List<Guid> AccessibleFarmIds { get; init; }
}

public class MeQueryHandler : IRequestHandler<MeQuery, BaseResponse<MeQueryResponse>>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IS3Service _s3Service;

    public MeQueryHandler(IUserDataResolver userDataResolver, IUserRepository userRepository, IMapper mapper,
        IS3Service s3Service)
    {
        _userDataResolver = userDataResolver;
        _userRepository = userRepository;
        _mapper = mapper;
        _s3Service = s3Service;
    }

    public async Task<BaseResponse<MeQueryResponse>> Handle(MeQuery request, CancellationToken ct)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.UserNotFound();
        var user = await _userRepository.SingleOrDefaultAsync(new UserByIdSpec(userId), ct) ??
                   throw DomainException.UserNotFound();

        var response = _mapper.Map<MeQueryResponse>(user);
        if (user.AvatarPath is not null)
        {
            response.AvatarPath = _s3Service.GeneratePreSignedUrl(FileType.Avatar, user.AvatarPath, "avatar.webp");
        }

        return BaseResponse.CreateResponse(response);
    }
}