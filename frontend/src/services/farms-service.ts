import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { FarmFormValues } from "../components/modals/farms/edit-farm-modal";
import type { HenhouseFormValues } from "../components/modals/farms/edit-henhouse-modal";
import type FarmRowModel from "../models/farms/farm-row-model";
import type { HouseRowModel } from "../models/farms/house-row-model";
import type LatestCycle from "../models/farms/latest-cycle";
import type { CycleSettingsData } from "../pages/settings/cycle-settings";
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

  public static async updateFarmAsync(id: string, data: FarmFormValues) {
    return await AxiosWrapper.patch(ApiUrl.UpdateFarm(id), data);
  }

  public static async deleteFarmAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteFarm(id));
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

  public static async updateHenhouseAsync(
    id: string,
    data: HenhouseFormValues
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateHenhouse(id), data);
  }

  public static async deleteHenhouseAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteHenhouse + "/" + id);
  }

  public static async updateFarmCycle(data: CycleSettingsData) {
    return await AxiosWrapper.post(ApiUrl.UpdateFarmCycle, data);
  }
}
