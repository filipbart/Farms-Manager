import ApiUrl from "../common/ApiUrl";
import AxiosWrapper from "../utils/axios/wrapper";

export interface SaleFieldsExtraRow {
  id: string;
  name: string;
}

export interface SaleFieldsExtraQueryResponse {
  fields: SaleFieldsExtraRow[];
}

export class SalesSettingsService {
  public static async getSaleFieldsExtra() {
    return await AxiosWrapper.get<SaleFieldsExtraQueryResponse>(
      ApiUrl.SalesSettings
    );
  }

  public static async addSaleFieldExtra(fields: string[]) {
    return await AxiosWrapper.post(ApiUrl.AddSaleFieldExtra, {
      fields,
    });
  }

  public static async deleteSaleFieldExtra(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteSaleFieldExtra + "/" + id);
  }
}
