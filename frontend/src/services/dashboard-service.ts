import ApiUrl from "../common/ApiUrl";
import type {
  DashboardStatsResponse,
  DashboardNotificationsResponse,
  DashboardChartsResponse,
  DashboardExpensesPieChart,
} from "../models/dashboard/dashboard";
import type {
  DashboardDictionary,
  DashboardFilters,
} from "../models/dashboard/dashboard-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export class DashboardService {
  /**
   * Pobiera słowniki (fermy, cykle) do filtrów na dashboardzie.
   */
  public static async getDictionaries() {
    return await AxiosWrapper.get<DashboardDictionary>(
      ApiUrl.DashboardDictionary
    );
  }

  /**
   * Pobiera główne statystyki (KPI) oraz statusy kurników.
   * @param filters - filtry (ferma, cykl, zakres dat).
   */
  public static async getStats(filters: DashboardFilters) {
    return await AxiosWrapper.get<DashboardStatsResponse>(
      ApiUrl.DashboardStats,
      {
        ...filters,
      }
    );
  }

  /**
   * Pobiera listę powiadomień.
   * @param filters - filtry (ferma).
   */
  public static async getNotifications(filters: DashboardFilters) {
    return await AxiosWrapper.get<DashboardNotificationsResponse>(
      ApiUrl.DashboardNotifications,
      { ...filters }
    );
  }

  /**
   * Pobiera dane do wszystkich wykresów liniowych (FCR, EWW, Zużycie gazu, Straty)
   * @param filters - filtry (ferma).
   */
  public static async getCharts(filters: DashboardFilters) {
    return await AxiosWrapper.get<DashboardChartsResponse>(
      ApiUrl.DashboardCharts,
      {
        ...filters,
      }
    );
  }

  /**
   * Pobiera dane do wykresu kołowego struktury wydatków.
   * @param filters - filtry (ferma, zakres dat).
   */
  public static async getExpensesPieChart(filters: DashboardFilters) {
    return await AxiosWrapper.get<DashboardExpensesPieChart>(
      ApiUrl.DashboardExpensesPieChart,
      { ...filters }
    );
  }
}
