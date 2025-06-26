import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type FarmRowModel from "../models/farms/farm-row-model";
import type { HouseRowModel } from "../models/farms/house-row-model";
import type LatestCycle from "../models/farms/latest-cycle";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AddFarmFormData {
  name: string;
  prodNumber: string;
  nip: string;
  address: string;
}

export interface AddHenhouseFormData {
  farmId: string;
  name: string;
  code: string;
  area: number;
  desc: string;
}

export class FarmsService {
  public static async getFarmsAsync() {
    return await AxiosWrapper.get<PaginateModel<FarmRowModel>>(ApiUrl.Farms);
  }

  public static async getLatestCycle(farmId: string) {
    return await AxiosWrapper.get<LatestCycle>(
      ApiUrl.Farms + "/" + farmId + ApiUrl.LatestCycle
    );
  }

  public static async addFarmAsync(data: AddFarmFormData) {
    return await AxiosWrapper.post(ApiUrl.AddFarm, data);
  }

  public static async deleteFarmAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteFarm + "/" + id);
  }

  public static async getFarmHousesAsync(farmId: string) {
    return await AxiosWrapper.get<PaginateModel<HouseRowModel>>(
      ApiUrl.Farms + "/" + farmId + "/henhouses"
    );
  }

  public static async addHenhouseAsync(data: AddHenhouseFormData) {
    return await AxiosWrapper.post(
      ApiUrl.Farms + "/" + data.farmId + "/add-henhouse",
      data
    );
  }

  public static async deleteHenhouseAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteHenhouse + "/" + id);
  }
}
