import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { SaleListModel, SaleType } from "../models/sales/sales";
import type { SalesDictionary } from "../models/sales/sales-dictionary";
import type { SalesFilterPaginationModel } from "../models/sales/sales-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AddSaleData {
  saleType: SaleType;
  farmId: string;
  cycleId: string;
  saleDate: string;
  slaughterhouseId: string;
  entries: {
    henhouseId: string;
    quantity: number;
    weight: number;
    confiscatedCount: number;
    confiscatedWeight: number;
    deadCount: number;
    deadWeight: number;
    farmerWeight: number;
    basePrice: number;
    priceWithExtras: number;
    comment?: string;
    otherExtras?: {
      name: string;
      value: number;
    }[];
  }[];
}

export interface AddNewSaleResponse {
  internalGroupId: string;
}

export class SalesService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<SalesDictionary>(ApiUrl.SalesDict);
  }

  public static async getSales(filters: SalesFilterPaginationModel) {
    return await AxiosWrapper.get<PaginateModel<SaleListModel>>(ApiUrl.Sales, {
      ...filters,
    });
  }

  public static async addNewSale(data: AddSaleData) {
    return await AxiosWrapper.post<AddNewSaleResponse>(
      ApiUrl.Sales + "/add",
      data
    );
  }

  public static async updateSale(saleId: string, payload: any) {
    return await AxiosWrapper.patch(ApiUrl.UpdateSale + "/" + saleId, payload);
  }

  public static async sendToIrzPlus(payload: {
    internalGroupId?: string;
    saleId?: string;
  }) {
    return await AxiosWrapper.post(ApiUrl.SaleSendToIrz, payload);
  }
}
