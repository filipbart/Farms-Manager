using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.FeedAggregate.Interfaces;
using FarmsManager.Domain.Exceptions;
using MediatR;

namespace FarmsManager.Application.Commands.Feeds.Deliveries;

public record MarkPaymentAsCompletedCommandDto
{
    public string Comment { get; init; }
}

public record MarkPaymentAsCompletedCommand(Guid Id, MarkPaymentAsCompletedCommandDto Data) 
    : IRequest<EmptyBaseResponse>;

public class MarkPaymentAsCompletedCommandHandler 
    : IRequestHandler<MarkPaymentAsCompletedCommand, EmptyBaseResponse>
{
    private readonly IUserDataResolver _userDataResolver;
    private readonly IFeedPaymentRepository _feedPaymentRepository;

    public MarkPaymentAsCompletedCommandHandler(
        IUserDataResolver userDataResolver,
        IFeedPaymentRepository feedPaymentRepository)
    {
        _userDataResolver = userDataResolver;
        _feedPaymentRepository = feedPaymentRepository;
    }

    public async Task<EmptyBaseResponse> Handle(
        MarkPaymentAsCompletedCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();
        
        var feedPayment = await _feedPaymentRepository.GetAsync(
            new FeedPaymentByIdSpec(request.Id), 
            cancellationToken);

        feedPayment.MarkAsCompleted(request.Data.Comment);
        
        await _feedPaymentRepository.UpdateAsync(feedPayment, cancellationToken);

        return new EmptyBaseResponse();
    }
}
