import ApiUrl from "../../common/ApiUrl";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type { ProductionDataFilterPaginationModel } from "../../models/production-data/production-data-filters";
import type {
  AddWeighingData,
  GetHatcheryParams,
  GetHatcheryResponse,
  ProductionDataWeighingListModel,
  UpdateWeighingData,
  WeightStandardRowModel,
} from "../../models/production-data/weighings";
import AxiosWrapper from "../../utils/axios/wrapper";

export class ProductionDataWeighingsService {
  /**
   * Pobiera listę wpisów o ważeniach
   */
  public static async getWeighings(
    filters: ProductionDataFilterPaginationModel
  ) {
    return await AxiosWrapper.get<
      PaginateModel<ProductionDataWeighingListModel>
    >(ApiUrl.ProductionDataWeighings, { ...filters });
  }

  /**
   * Pobiera domyślną wylęgarnię na podstawie wstawienia
   */
  public static async getHatcheryForWeighing(params: GetHatcheryParams) {
    return await AxiosWrapper.get<GetHatcheryResponse>(
      ApiUrl.GetHatcheryForWeighing,
      params
    );
  }

  /**
   * Dodaje nowy wpis (pierwsze ważenie)
   */
  public static async addWeighing(data: AddWeighingData) {
    return await AxiosWrapper.post(ApiUrl.AddProductionDataWeighing, data);
  }

  /**
   * Aktualizuje istniejący wpis (dodaje kolejne ważenie)
   */
  public static async updateWeighing(id: string, payload: UpdateWeighingData) {
    return await AxiosWrapper.patch(
      ApiUrl.UpdateProductionDataWeighing(id),
      payload
    );
  }

  /**
   * Usuwa istniejący wpis
   */
  public static async deleteWeighing(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteProductionDataWeighing(id));
  }

  /**
   * Pobiera listę norm wagowych
   */
  public static async getStandards() {
    return await AxiosWrapper.get<WeightStandardRowModel[]>(
      ApiUrl.WeightStandards
    );
  }

  /**
   * Usuwa istniejącą normę wagową
   * @param standardId - ID normy do usunięcia
   */
  public static async deleteStandard(standardId: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteWeightStandard(standardId));
  }

  /**
   * Dodaje nowe normy wagowe
   * @param data - Tablica obiektów z nowymi normami
   */
  public static async addStandards(data: { day: number; weight: number }[]) {
    return await AxiosWrapper.post(ApiUrl.AddWeightStandards, data);
  }
}
