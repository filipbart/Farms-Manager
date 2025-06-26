import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
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

  public static async deleteHatcheryAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteHatchery + "/" + id);
  }
}
