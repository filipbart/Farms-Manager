import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { ProductionAnalysisRowModel } from "../models/summary/production-analysis";
import type {
  ProductionAnalysisDictionary,
  ProductionAnalysisFilterPaginationModel,
} from "../models/summary/production-analysis-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export class SummaryService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<ProductionAnalysisDictionary>(
      ApiUrl.SummaryDictionary
    );
  }

  public static async getProductionAnalysisColumns() {
    return await AxiosWrapper.get(ApiUrl.SummaryProductionAnalysisColumns);
  }

  public static async getProductionAnalysis(
    filters: ProductionAnalysisFilterPaginationModel
  ) {
    return await AxiosWrapper.get<PaginateModel<ProductionAnalysisRowModel>>(
      ApiUrl.SummaryProductionAnalysis,
      { ...filters }
    );
  }
}
