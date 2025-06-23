import ApiUrl from "../common/ApiUrl";
import type LatestCycle from "../models/farms/latest-cycle";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AddCycleData {
  farmId: string;
  identifier: number;
  year: number;
}

export interface AddInsertionData {
  farmId: string;
  henhouseId: string;
  identifierId: string;
  insertionDate: Date;
  quantity: number;
  hatcheryId: string;
  bodyWeight: number;
}

export class InsertionsService {
  public static async addNewInsertion(data: AddInsertionData) {
    return await AxiosWrapper.post(ApiUrl.Insertions + "/add", data);
  }

  public static async addNewCycle(data: AddCycleData) {
    return await AxiosWrapper.post<LatestCycle>(
      ApiUrl.Insertions + "/add-cycle",
      data
    );
  }
}
