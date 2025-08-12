import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { ProductionAnalysisRowModel } from "../models/summary/production-analysis";
import type {
  AnalysisDictionary,
  AnalysisFilterPaginationModel,
} from "../models/summary/analysis-filters";
import AxiosWrapper from "../utils/axios/wrapper";
import type { FinancialAnalysisRowModel } from "../models/summary/financial-analysis";

export class SummaryService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<AnalysisDictionary>(ApiUrl.SummaryDictionary);
  }

  public static async getProductionAnalysis(
    filters: AnalysisFilterPaginationModel
  ) {
    return await AxiosWrapper.get<PaginateModel<ProductionAnalysisRowModel>>(
      ApiUrl.SummaryProductionAnalysis,
      { ...filters }
    );
  }

  public static async getFinancialAnalysis(
    filters: AnalysisFilterPaginationModel
  ) {
    return await AxiosWrapper.get<PaginateModel<FinancialAnalysisRowModel>>(
      ApiUrl.SummaryFinancialAnalysis,
      { ...filters }
    );
  }
}
