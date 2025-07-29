import ApiUrl from "../../common/ApiUrl";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type { ProductionDataFilterPaginationModel } from "../../models/production-data/production-data-filters";
import type {
  AddRemainingFeedData,
  ProductionDataRemainingFeedListModel,
  UpdateRemainingFeedData,
} from "../../models/production-data/remaining-feed/remaining-feed";
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

export class ProductionDataRemainingFeedService {
  /**
   * Pobiera listę wpisów o pozostałej paszy
   */
  public static async getRemainingFeeds(
    filters: ProductionDataFilterPaginationModel
  ) {
    return await AxiosWrapper.get<
      PaginateModel<ProductionDataRemainingFeedListModel>
    >(ApiUrl.ProductionDataRemainingFeed, { ...filters });
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

  /**
   * Dodaje nowy wpis
   */
  public static async addRemainingFeed(data: AddRemainingFeedData) {
    return await AxiosWrapper.post(ApiUrl.AddProductionDataRemainingFeed, data);
  }

  /**
   * Aktualizuje istniejący wpis
   */
  public static async updateRemainingFeed(
    id: string,
    payload: UpdateRemainingFeedData
  ) {
    return await AxiosWrapper.patch(
      ApiUrl.UpdateProductionDataRemainingFeed(id),
      payload
    );
  }

  /**
   * Usuwa istniejący wpis
   */
  public static async deleteRemainingFeed(id: string) {
    return await AxiosWrapper.delete(
      ApiUrl.DeleteProductionDataRemainingFeed(id)
    );
  }
}
