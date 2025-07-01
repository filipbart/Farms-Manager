import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { SlaughterhouseRowModel } from "../models/slaughterhouses/slaughterhouse-row-model";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AddSlaughterhouseFormData {
  name: string;
  prodNumber: string;
  fullName: string;
  nip: string;
  address: string;
}

export class SlaughterhousesService {
  public static async getAllSlaughterhouses() {
    return await AxiosWrapper.get<PaginateModel<SlaughterhouseRowModel>>(
      ApiUrl.Slaughterhouses
    );
  }

  public static async addSlaughterhouseAsync(data: AddSlaughterhouseFormData) {
    return await AxiosWrapper.post(ApiUrl.AddSlaughterhouse, data);
  }

  public static async deleteSlaughterhouseAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteSlaughterhouse + "/" + id);
  }
}
