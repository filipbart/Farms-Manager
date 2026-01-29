using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.InvoiceFarmAssignmentRules;

public record GetInvoiceFarmAssignmentRulesQuery : IRequest<BaseResponse<List<InvoiceFarmAssignmentRuleResponse>>>;
