import ApiUrl from "../common/ApiUrl";
import type { SaleType } from "../models/sales/sales";
import type { SalesDictionary } from "../models/sales/sales-dictionary";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AddSaleData {
  saleType: SaleType;
  farmId: string;
  cycleId: string;
  saleDate: string;
  entries: {
    henhouseId: string;
    slaughterhouseId: string;
    quantity: number;
    weight: number;
    confiscatedCount: number;
    confiscatedWeight: number;
    deadCount: number;
    deadWeight: number;
    farmerWeight: number;
  }[];
  basePrice: number;
  priceWithExtras: number;
  comment?: string;
  otherExtras?: {
    name: string;
    value: string;
  }[];
}

export interface AddNewSaleResponse {
  internalGroupId: string;
}

export class SalesService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<SalesDictionary>(ApiUrl.SalesDict);
  }

  public static async addNewSale(data: AddSaleData) {
    return await AxiosWrapper.post<AddNewSaleResponse>(
      ApiUrl.Sales + "/add",
      data
    );
  }
}
