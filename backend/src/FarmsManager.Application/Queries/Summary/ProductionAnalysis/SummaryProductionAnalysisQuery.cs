using FarmsManager.Application.Common.Responses;
using FarmsManager.Application.Queries.Insertions;
using MediatR;

namespace FarmsManager.Application.Queries.Summary.ProductionAnalysis;

public record SummaryProductionAnalysisQuery(GetInsertionsQueryFilters Filters)
    : IRequest<BaseResponse<SummaryProductionAnalysisQueryResponse>>;