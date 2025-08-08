import ApiUrl from "../../common/ApiUrl";
import type {
  IrzSummaryData,
  AddFallenStocksData,
  FallenStockEditableEntry,
  GetAvailableHenhousesFallenStocks,
  GetAvailableHenhousesFallenStocksResponse,
  GetFallenStockEditData,
} from "../../models/fallen-stocks/fallen-stocks";
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

  public static async getAvailableHenhouses(
    query: GetAvailableHenhousesFallenStocks
  ) {
    return await AxiosWrapper.get<GetAvailableHenhousesFallenStocksResponse>(
      ApiUrl.FallenStocksHenhouses,
      { ...query }
    );
  }

  public static async addNewFallenStocks(data: AddFallenStocksData) {
    return await AxiosWrapper.post(ApiUrl.AddFallenStocks, data);
  }

  public static async deleteFallenStocks(internalGroupId: string) {
    return await AxiosWrapper.delete(
      ApiUrl.DeleteFallenStocks(internalGroupId)
    );
  }

  public static async getFallenStocksDataForEdit(internalGroupId: string) {
    return await AxiosWrapper.get<GetFallenStockEditData>(
      ApiUrl.GetFallenStocksDataForEdit(internalGroupId)
    );
  }

  public static async updateFallenStocks(
    internalGroupId: string,
    data: FallenStockEditableEntry[]
  ) {
    return await AxiosWrapper.patch(
      ApiUrl.UpdateFallenStocks(internalGroupId),
      { entries: data }
    );
  }

  public static async getIrzSummaryData(farmId: string, cycle: string) {
    return await AxiosWrapper.get<IrzSummaryData>(ApiUrl.GetIrzSummaryData, {
      farmId,
      cycle,
    });
  }
}
