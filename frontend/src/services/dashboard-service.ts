import ApiUrl from "../common/ApiUrl";
import type {
  DashboardStatsResponse,
  DashboardNotificationsResponse,
  DashboardEwwChart,
  DashboardFcrChart,
  DashboardFlockLossChart,
  DashboardExpensesPieChart,
  DashboardGasConsumptionChart,
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
   * Pobiera dane do wykresu EWW (Europejski Wskaźnik Wydajności).
   * @param filters - filtry (ferma).
   */
  public static async getEwwChart(filters: DashboardFilters) {
    return await AxiosWrapper.get<DashboardEwwChart>(ApiUrl.DashboardEwwChart, {
      ...filters,
    });
  }

  /**
   * Pobiera dane do wykresu FCR (Współczynnik Konwersji Paszy).
   * @param filters - filtry (ferma).
   */
  public static async getFcrChart(filters: DashboardFilters) {
    return await AxiosWrapper.get<DashboardFcrChart>(ApiUrl.DashboardFcrChart, {
      ...filters,
    });
  }

  /**
   * Pobiera dane do wykresu śmiertelności stada.
   * @param filters - filtry (ferma).
   */
  public static async getFlockLossChart(filters: DashboardFilters) {
    return await AxiosWrapper.get<DashboardFlockLossChart>(
      ApiUrl.DashboardFlockLossChart,
      { ...filters }
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

  /**
   * Pobiera dane do wykresu zużycia gazu.
   * @param filters - filtry (ferma).
   */
  public static async getGasConsumptionChart(filters: DashboardFilters) {
    return await AxiosWrapper.get<DashboardGasConsumptionChart>(
      ApiUrl.DashboardGasConsumptionChart,
      { ...filters }
    );
  }
}
