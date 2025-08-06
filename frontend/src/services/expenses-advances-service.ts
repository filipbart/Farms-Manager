import ApiUrl from "../common/ApiUrl";
import type {
  AddAdvanceCategory,
  AdvanceCategoryModel,
} from "../models/expenses/advances/categories";
import AxiosWrapper from "../utils/axios/wrapper";

export class ExpensesAdvancesService {
  public static async getAdvanceCategories() {
    return await AxiosWrapper.get<AdvanceCategoryModel[]>(
      ApiUrl.ExpensesAdvancesCategories
    );
  }

  public static async addAdvanceCategories(data: AddAdvanceCategory[]) {
    return await AxiosWrapper.post(ApiUrl.AddAdvanceCategory, data);
  }

  public static async deleteAdvanceCategory(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteAdvanceCategory(id));
  }
}
