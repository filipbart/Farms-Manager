using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.InvoiceAssignmentRules;

public record GetInvoiceAssignmentRulesQuery : IRequest<BaseResponse<List<InvoiceAssignmentRuleDto>>>;
