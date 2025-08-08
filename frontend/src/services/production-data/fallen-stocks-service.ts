import ApiUrl from "../../common/ApiUrl";
import type {
  FallenStockFilterModel,
  FallenStocksDictionary,
} from "../../models/fallen-stocks/fallen-stocks-filters";
import type { FallenStockTableViewModel } from "../../models/fallen-stocks/fallen-stocks-view-model";
import AxiosWrapper from "../../utils/axios/wrapper";

export class FallenStockService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<FallenStocksDictionary>(
      ApiUrl.FallenStocksDictionary
    );
  }

  public static async getFallenStocks(filters: FallenStockFilterModel) {
    return await AxiosWrapper.get<FallenStockTableViewModel>(
      ApiUrl.FallenStocks,
      { ...filters }
    );
  }
}
