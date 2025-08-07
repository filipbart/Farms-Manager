import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  AddUtilizationPlantFormData,
  UtilizationPlantRowModel,
} from "../models/utilization-plants/utilization-plants";
import AxiosWrapper from "../utils/axios/wrapper";

export class UtilizationPlantsService {
  public static async getAllUtilizationPlants() {
    return await AxiosWrapper.get<PaginateModel<UtilizationPlantRowModel>>(
      ApiUrl.UtilizationPlants
    );
  }

  public static async addUtilizationPlantAsync(
    data: AddUtilizationPlantFormData
  ) {
    return await AxiosWrapper.post(ApiUrl.AddUtilizationPlant, data);
  }

  public static async deleteUtilizationPlantAsync(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteUtilizationPlant + "/" + id);
  }
}
