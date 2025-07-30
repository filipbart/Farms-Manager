import ApiUrl from "../../common/ApiUrl";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type { ProductionDataFilterPaginationModel } from "../../models/production-data/production-data-filters";
import type {
  AddTransferFeedData,
  ProductionDataTransferFeedListModel,
  UpdateTransferFeedData,
} from "../../models/production-data/transfer-feed";
import AxiosWrapper from "../../utils/axios/wrapper";

export class ProductionDataTransferFeedService {
  /**
   * Pobiera listę wpisów o przeniesieniach paszy
   */
  public static async getFeedTransfers(
    filters: ProductionDataFilterPaginationModel
  ) {
    return await AxiosWrapper.get<
      PaginateModel<ProductionDataTransferFeedListModel>
    >(ApiUrl.ProductionDataTransferFeed, { ...filters });
  }

  /**
   * Dodaje nowy wpis
   */
  public static async addFeedTransfer(data: AddTransferFeedData) {
    return await AxiosWrapper.post(ApiUrl.AddProductionDataTransferFeed, data);
  }

  /**
   * Aktualizuje istniejący wpis
   */
  public static async updateFeedTransfer(
    id: string,
    payload: UpdateTransferFeedData
  ) {
    return await AxiosWrapper.patch(
      ApiUrl.UpdateProductionDataTransferFeed(id),
      payload
    );
  }

  /**
   * Usuwa istniejący wpis
   */
  public static async deleteFeedTransfer(id: string) {
    return await AxiosWrapper.delete(
      ApiUrl.DeleteProductionDataTransferFeed(id)
    );
  }
}
