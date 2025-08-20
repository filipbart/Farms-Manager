import ApiUrl from "../common/ApiUrl";
import type {
  DashboardDictionary,
  DashboardFilters,
} from "../models/dashboard/dashboard-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export class DashboardService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<DashboardDictionary>(
      ApiUrl.DashboardDictionary
    );
  }

  public static async getFeedsPrices(filters: DashboardFilters) {
    //TODO response
    return await AxiosWrapper.get(ApiUrl.Dashboard, {
      ...filters,
    });
  }
}
