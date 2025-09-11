import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { HatcheryFormValues } from "../components/modals/hatcheries/edit-hatchery-modal";
import type {
  GetNotesResponse,
  HatcheryNoteData,
} from "../models/hatcheries/hatcheries-notes";
import type {
  AddHatcheryPriceFormData,
  EditHatcherPriceFormData as EditHatcheryPriceFormData,
  HatcheriesNames,
  HatcheryPriceListModel,
} from "../models/hatcheries/hatcheries-prices";
import type { HatcheriesPricesFilterPaginationModel } from "../models/hatcheries/hatcheries-prices-filters";
import type { HatcheryRowModel } from "../models/hatcheries/hatchery-row-model";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AddHatcheryFormData {
  name: string;
  prodNumber: string;
  fullName: string;
  nip: string;
  address: string;
}

export class HatcheriesService {
  public static async getAllHatcheries() {
    return await AxiosWrapper.get<PaginateModel<HatcheryRowModel>>(
      ApiUrl.Hatcheries
    );
  }

  public static async addHatcheryAsync(data: AddHatcheryFormData) {
    return await AxiosWrapper.post(ApiUrl.AddHatchery, data);
  }

  public static async updateHatcheryAsync(
    id: string,
    data: HatcheryFormValues
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateHatchery(id), data);
  }

  public static async deleteHatcheryAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteHatchery(id));
  }

  public static async getPricesNames() {
    return await AxiosWrapper.get<HatcheriesNames>(
      ApiUrl.HatcheriesPricesNames
    );
  }

  public static async deleteHatcheryName(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteHatcheryPriceName(id));
  }

  public static async addHatcheryName(name: string) {
    return await AxiosWrapper.post(ApiUrl.AddHatcheryPriceName, { name });
  }

  public static async getHatcheriesPrices(
    filters: HatcheriesPricesFilterPaginationModel
  ) {
    return await AxiosWrapper.get<PaginateModel<HatcheryPriceListModel>>(
      ApiUrl.HatcheriesPrices,
      { ...filters }
    );
  }

  public static async addHatcheryPrice(data: AddHatcheryPriceFormData) {
    return await AxiosWrapper.post(ApiUrl.AddHatcheryPrice, data);
  }

  public static async editHatcheryPrice(
    id: string,
    data: EditHatcheryPriceFormData
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateHatcheryPrice(id), data);
  }

  public static async deleteHatcheryPrice(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteHatcheryPrice(id));
  }

  public static async getNotes() {
    return await AxiosWrapper.get<GetNotesResponse>(ApiUrl.HatcheriesNotes);
  }

  public static async updateNotesOrder(orderedIds: string[]) {
    return await AxiosWrapper.patch(ApiUrl.UpdateHatcheryNoteOrder, {
      orderedIds,
    });
  }

  public static async addNote(data: HatcheryNoteData) {
    return await AxiosWrapper.post(ApiUrl.AddHatcheryNote, data);
  }

  public static async updateNote(id: string, data: HatcheryNoteData) {
    return await AxiosWrapper.patch(ApiUrl.UpdateHatcheryNote(id), data);
  }

  public static async deleteNote(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteHatcheryNote(id));
  }
}
