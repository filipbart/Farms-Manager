using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Aggregates.AccountingAggregate.Enums;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting;

public record GetInvoiceAuditLogsQuery(Guid InvoiceId) : IRequest<BaseResponse<List<InvoiceAuditLogItemDto>>>;

public record InvoiceAuditLogItemDto
{
    public Guid Id { get; init; }
    public string Action { get; init; }
    public string ActionDescription { get; init; }
    public string PreviousStatus { get; init; }
    public string NewStatus { get; init; }
    public string UserName { get; init; }
    public string Comment { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class GetInvoiceAuditLogsQueryHandler : IRequestHandler<GetInvoiceAuditLogsQuery, BaseResponse<List<InvoiceAuditLogItemDto>>>
{
    private readonly IInvoiceAuditService _auditService;
    private readonly IUserDataResolver _userDataResolver;

    public GetInvoiceAuditLogsQueryHandler(
        IInvoiceAuditService auditService,
        IUserDataResolver userDataResolver)
    {
        _auditService = auditService;
        _userDataResolver = userDataResolver;
    }

    public async Task<BaseResponse<List<InvoiceAuditLogItemDto>>> Handle(
        GetInvoiceAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        _ = _userDataResolver.GetUserId() ?? throw DomainException.Unauthorized();

        var auditLogs = await _auditService.GetInvoiceAuditHistoryAsync(
            request.InvoiceId,
            cancellationToken
        );

        var result = auditLogs.Select(a => new InvoiceAuditLogItemDto
        {
            Id = a.Id,
            Action = a.Action.ToString(),
            ActionDescription = a.Action.ToString(),
            PreviousStatus = a.PreviousStatus?.ToString(),
            NewStatus = a.NewStatus?.ToString(),
            UserName = a.UserName,
            Comment = a.Comment,
            CreatedAt = a.DateCreatedUtc
        }).ToList();

        return BaseResponse.CreateResponse(result);
    }
}
