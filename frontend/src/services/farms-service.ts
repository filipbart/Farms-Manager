import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type FarmRowModel from "../models/farms/farm-row-model";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AddFarmFormData {
  name: string;
  nip: string;
  address: string;
}

export class FarmsService {
  public static async getFarmsAsync() {
    return await AxiosWrapper.get<PaginateModel<FarmRowModel>>(ApiUrl.Farms);
  }

  public static async addFarmAsync(data: AddFarmFormData) {
    return await AxiosWrapper.post(ApiUrl.AddFarm, data);
  }

  public static async deleteFarmAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteFarm + "/" + id);
  }
}
