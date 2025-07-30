import ApiUrl from "../../common/ApiUrl";
import type { ProductionDataDictionary } from "../../models/production-data/production-data-filters";
import AxiosWrapper from "../../utils/axios/wrapper";

interface CalculateValueParams {
  cycleId: string;
  henhouseId: string;
  feedName: string;
  tonnage: number;
}

interface CalculatedValueResponse {
  value: number;
}

export class ProductionDataService {
  /**
   * Pobiera słowniki potrzebne do filtrów
   */
  public static async getDictionaries() {
    return await AxiosWrapper.get<ProductionDataDictionary>(
      ApiUrl.ProductionDataDictionary
    );
  }

  /**
   * Oblicza wartość pozostałej paszy
   */
  public static async calculateValue(params: CalculateValueParams) {
    return await AxiosWrapper.get<CalculatedValueResponse>(
      ApiUrl.CalculateRemainingFeedValue,
      params
    );
  }
}
