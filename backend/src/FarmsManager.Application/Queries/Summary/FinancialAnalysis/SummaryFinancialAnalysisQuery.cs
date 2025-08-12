using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Insertions;
using MediatR;

namespace FarmsManager.Application.Queries.Summary.FinancialAnalysis;

public record SummaryFinancialAnalysisQuery(GetInsertionsQueryFilters Filters)
    : IRequest<BaseResponse<SummaryFinancialAnalysisQueryResponse>>;