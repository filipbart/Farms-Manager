import ApiUrl from "../../common/ApiUrl";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type {
  AddFlockLossMeasureData,
  ProductionDataFlockLossListModel,
  UpdateFlockLossData,
} from "../../models/production-data/flock-loss";
import type {
  ProductionDataFlockLossDictionary,
  ProductionDataFlockLossFilterPaginationModel,
} from "../../models/production-data/flock-loss-filters";
import AxiosWrapper from "../../utils/axios/wrapper";

export class ProductionDataFlockLossService {
  /**
   * Pobiera listę wpisów o pomiarach upadków i wybrakowań
   */
  public static async getFlockLosses(
    filters: ProductionDataFlockLossFilterPaginationModel
  ) {
    return await AxiosWrapper.get<
      PaginateModel<ProductionDataFlockLossListModel>
    >(ApiUrl.ProductionDataFlockLosses, { ...filters });
  }

  /**
   * Pobiera słowniki potrzebne do filtrów
   */
  public static async getDictionaries() {
    return await AxiosWrapper.get<ProductionDataFlockLossDictionary>(
      ApiUrl.ProductionDataFlockLossDictionary
    );
  }

  /**
   * Dodaje nowy wpis pomiaru
   */
  public static async addFlockLossMeasure(data: AddFlockLossMeasureData) {
    return await AxiosWrapper.post(ApiUrl.AddProductionDataFlockLoss, data);
  }

  /**
   * Aktualizuje istniejący wpis pomiaru
   */
  public static async updateFlockLoss(
    id: string,
    payload: UpdateFlockLossData
  ) {
    return await AxiosWrapper.patch(
      ApiUrl.UpdateProductionDataFlockLoss(id),
      payload
    );
  }

  /**
   * Usuwa istniejący wpis
   */
  public static async deleteFlockLoss(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteProductionDataFlockLoss(id));
  }
}
