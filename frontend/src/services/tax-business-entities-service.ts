import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  AddTaxBusinessEntityFormData,
  TaxBusinessEntityRowModel,
  UpdateTaxBusinessEntityFormData,
} from "../models/data/tax-business-entity";
import AxiosWrapper from "../utils/axios/wrapper";

export class TaxBusinessEntitiesService {
  public static async getAllAsync() {
    return await AxiosWrapper.get<PaginateModel<TaxBusinessEntityRowModel>>(
      ApiUrl.TaxBusinessEntities
    );
  }

  public static async addAsync(data: AddTaxBusinessEntityFormData) {
    return await AxiosWrapper.post(ApiUrl.AddTaxBusinessEntity, data);
  }

  public static async updateAsync(
    id: string,
    data: UpdateTaxBusinessEntityFormData
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateTaxBusinessEntity(id), data);
  }

  public static async deleteAsync(id: string) {
    return await AxiosWrapper.post(ApiUrl.DeleteTaxBusinessEntity(id));
  }
}
