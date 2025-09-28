import ApiUrl from "../../common/ApiUrl";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type {
  AddWeighingData,
  GetHatcheryParams,
  GetHatcheryResponse,
  GetWeightStandardsQueryResponse,
  ProductionDataWeighingListModel,
  UpdateWeighingData,
} from "../../models/production-data/weighings";
import type {
  ProductionDataWeighingsDictionary,
  ProductionDataWeighingsFilterPaginationModel,
} from "../../models/production-data/weighings-filters";
import AxiosWrapper from "../../utils/axios/wrapper";

export class ProductionDataWeighingsService {
  /**
   * Pobiera listę wpisów o ważeniach
   */
  public static async getWeighings(
    filters: ProductionDataWeighingsFilterPaginationModel
  ) {
    return await AxiosWrapper.get<
      PaginateModel<ProductionDataWeighingListModel>
    >(ApiUrl.ProductionDataWeighings, { ...filters });
  }

  /**
   * Pobiera słowniki potrzebne do filtrów
   */
  public static async getDictionaries() {
    return await AxiosWrapper.get<ProductionDataWeighingsDictionary>(
      ApiUrl.ProductionDataWeighingsDictionary
    );
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
   * Pobiera listę norm wagowych
   */
  public static async getStandards() {
    return await AxiosWrapper.get<GetWeightStandardsQueryResponse>(
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
  // PO
  public static async addStandards(data: { day: number; weight: number }[]) {
    return await AxiosWrapper.post(ApiUrl.AddWeightStandards, {
      standards: data,
    });
  }
}
