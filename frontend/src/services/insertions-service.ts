import ApiUrl from "../common/ApiUrl";
import type LatestCycle from "../models/farms/latest-cycle";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AddCycleData {
  farmId: string;
  identifier: number;
  year: number;
}

export class InsertionsService {
  public static async addNewCycle(data: AddCycleData) {
    return await AxiosWrapper.post<LatestCycle>(
      ApiUrl.Insertions + "/add-cycle",
      data
    );
  }
}
