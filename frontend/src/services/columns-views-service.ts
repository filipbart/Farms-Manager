import ApiUrl from "../common/ApiUrl";
import AxiosWrapper from "../utils/axios/wrapper";

export enum ColumnViewType {
  SummaryProductionAnalysis = "SummaryProductionAnalysis",
  SummaryProductionFinancial = "SummaryProductionFinancial",
}
export interface ColumnViewRow {
  id: string;
  name: string;
  state: string;
  type: ColumnViewType;
}

export interface GetColumnsViewsResponse {
  items: ColumnViewRow[];
}

export interface AddColumnViewData {
  name: string;
  state: string;
  type: ColumnViewType;
}

export class ColumnsViewsService {
  public static async getColumnsViews(type: ColumnViewType) {
    return await AxiosWrapper.get<GetColumnsViewsResponse>(
      ApiUrl.ColumnsViews,
      { type }
    );
  }

  public static async addColumnView(data: AddColumnViewData) {
    return await AxiosWrapper.post(ApiUrl.AddColumnView, data);
  }

  public static async deleteColumnView(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteColumnView(id));
  }
}
