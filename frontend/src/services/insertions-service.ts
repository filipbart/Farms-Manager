import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type LatestCycle from "../models/farms/latest-cycle";
import type { InsertionDictionary } from "../models/insertions/insertion-dictionary";
import type InsertionListModel from "../models/insertions/insertions";
import type { InsertionsFilterPaginationModel } from "../models/insertions/insertions-filters";
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

  public static async getDictionaries() {
    return await AxiosWrapper.get<InsertionDictionary>(ApiUrl.InsertionsDict);
  }

  public static async getInsertions(filters: InsertionsFilterPaginationModel) {
    return await AxiosWrapper.get<PaginateModel<InsertionListModel>>(
      ApiUrl.Insertions,
      { ...filters }
    );
  }
}
