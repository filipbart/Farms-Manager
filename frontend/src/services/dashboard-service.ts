import ApiUrl from "../common/ApiUrl";
import type { GetDashboardDataQueryResponse } from "../models/dashboard/dashboard";
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

  public static async getDashboardData(filters: DashboardFilters) {
    return await AxiosWrapper.get<GetDashboardDataQueryResponse>(
      ApiUrl.Dashboard,
      { ...filters }
    );
  }
}
