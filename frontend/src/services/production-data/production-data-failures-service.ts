import ApiUrl from "../../common/ApiUrl";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type {
  AddProductionDataFailureData,
  ProductionDataFailureListModel,
  UpdateProductionDataFailureData,
} from "../../models/production-data/failures/failures";
import type { ProductionDataFilterPaginationModel } from "../../models/production-data/production-data-filters";
import AxiosWrapper from "../../utils/axios/wrapper";

export class ProductionDataFailuresService {
  /**
   * Pobiera listę wpisów o upadkach i wybrakowaniach
   */
  public static async getFailures(
    filters: ProductionDataFilterPaginationModel
  ) {
    return await AxiosWrapper.get<
      PaginateModel<ProductionDataFailureListModel>
    >(ApiUrl.ProductionDataFailures, { ...filters });
  }

  /**
   * Dodaje nowy wpis
   */
  public static async addFailure(data: AddProductionDataFailureData) {
    return await AxiosWrapper.post(ApiUrl.AddProductionDataFailure, data);
  }

  /**
   * Aktualizuje istniejący wpis
   */
  public static async updateFailure(
    failureId: string,
    payload: UpdateProductionDataFailureData
  ) {
    return await AxiosWrapper.patch(
      ApiUrl.UpdateProductionDataFailure(failureId),
      payload
    );
  }

  /**
   * Usuwa istniejący wpis
   */
  public static async deleteFailure(failureId: string) {
    return await AxiosWrapper.delete(
      ApiUrl.DeleteProductionDataFailure(failureId)
    );
  }
}
