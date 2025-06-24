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
  cycleId: string;
  insertionDate: string;
  entries: {
    henhouseId: string;
    hatcheryId: string;
    quantity: number;
    bodyWeight: number;
  }[];
}

export class InsertionsService {
  public static async addNewInsertion(data: AddInsertionData) {
    console.log(data);
    return await AxiosWrapper.post(ApiUrl.Insertions + "/add", data);
  }

  public static async addNewCycle(data: AddCycleData) {
    return await AxiosWrapper.post<LatestCycle>(
      ApiUrl.Insertions + "/add-cycle",
      data
    );
  }
}
