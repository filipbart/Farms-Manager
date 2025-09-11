import type { GridRowId } from "@mui/x-data-grid";
import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { HouseRowModel } from "../models/farms/house-row-model";
import type CycleDto from "../models/farms/latest-cycle";
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

export interface AddNewInsertionResponse {
  internalGroupId: string;
}

export interface AvailableHenhousesResponse {
  items: HouseRowModel[];
}

interface MarkAsReportedToWios {
  insertionIds: GridRowId[];
  comment: string;
}

interface SendToIrzPlusBulk {
  insertionIds: GridRowId[];
  comment: string;
}

export class InsertionsService {
  public static async addNewInsertion(data: AddInsertionData) {
    return await AxiosWrapper.post<AddNewInsertionResponse>(
      ApiUrl.Insertions + "/add",
      data
    );
  }

  public static async markAsReportedToWios(data: MarkAsReportedToWios) {
    return await AxiosWrapper.post(ApiUrl.InsertionMarkReportedToWios, data);
  }

  public static async updateInsertion(insertionId: string, payload: any) {
    return await AxiosWrapper.patch(
      ApiUrl.UpdateInsertion + "/" + insertionId,
      payload
    );
  }

  public static async deleteInsertion(insertionId: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteInsertion(insertionId));
  }

  public static async addNewCycle(data: AddCycleData) {
    return await AxiosWrapper.post<CycleDto>(
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

  public static async getAvailableHenhouses(farmId: string) {
    return await AxiosWrapper.get<AvailableHenhousesResponse>(
      ApiUrl.InsertionAvailableHenhouses,
      {
        farmId: farmId,
      }
    );
  }

  public static async sendToIrzPlus(payload: {
    internalGroupId?: string;
    insertionId?: string;
  }) {
    return await AxiosWrapper.post(ApiUrl.InsertionSendToIrz, payload);
  }

  public static async markAsSentToIrz(data: SendToIrzPlusBulk) {
    return await AxiosWrapper.post(ApiUrl.MarkAsSentToIrzPlus, data);
  }
}
