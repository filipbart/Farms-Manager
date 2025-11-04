using FarmsManager.Application.Common;
using FarmsManager.Application.Models.Summary;

namespace FarmsManager.Application.Queries.Summary.ProductionAnalysis;

public class SummaryProductionAnalysisQueryResponse : PaginationModel<SummaryProductionAnalysisRowDto>
{
    public SummaryProductionAnalysisSummaryDto Summary { get; init; }
}