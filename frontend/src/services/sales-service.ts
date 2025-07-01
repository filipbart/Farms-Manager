import ApiUrl from "../common/ApiUrl";
import type { SalesDictionary } from "../models/sales/sales-dictionary";
import AxiosWrapper from "../utils/axios/wrapper";

export class SalesService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<SalesDictionary>(ApiUrl.SalesDict);
  }
}
