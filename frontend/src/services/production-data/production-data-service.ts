import ApiUrl from "../../common/ApiUrl";
import type { ProductionDataDictionary } from "../../models/production-data/production-data-filters";
import AxiosWrapper from "../../utils/axios/wrapper";

export class ProductionDataService {
  /**
   * Pobiera słowniki potrzebne do filtrów
   */
  public static async getDictionaries() {
    return await AxiosWrapper.get<ProductionDataDictionary>(
      ApiUrl.ProductionDataDictionary
    );
  }
}
