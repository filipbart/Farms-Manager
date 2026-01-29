using FarmsManager.Application.Common.Responses;
using MediatR;

namespace FarmsManager.Application.Queries.Accounting.InvoiceModuleAssignmentRules;

public record GetInvoiceModuleAssignmentRulesQuery : IRequest<BaseResponse<List<InvoiceModuleAssignmentRuleDto>>>;
