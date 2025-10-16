import ApiUrl from "../common/ApiUrl";
import type {
  AddAdvanceCategory,
  AdvanceCategoryModel,
} from "../models/expenses/advances/categories";
import type {
  AddExpenseAdvance,
  GetExpensesAdvancesResponse,
  UpdateExpenseAdvance,
} from "../models/expenses/advances/expenses-advances";
import type { ExpensesAdvancesFilterPaginationModel } from "../models/expenses/advances/expenses-advances-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export class ExpensesAdvancesService {
  public static async getExpensesAdvances(
    employeeId: string,
    filters: ExpensesAdvancesFilterPaginationModel
  ) {
    return await AxiosWrapper.get<GetExpensesAdvancesResponse>(
      ApiUrl.ExpensesAdvancesDetails(employeeId),
      { ...filters }
    );
  }

  public static async addExpenseAdvance(
    employeeId: string,
    data: AddExpenseAdvance
  ) {
    const formData = new FormData();

    formData.append("type", data.type);

    data.entries.forEach((entry, index) => {
      formData.append(`Entries[${index}].date`, entry.date);
      formData.append(`Entries[${index}].name`, entry.name);
      formData.append(`Entries[${index}].amount`, String(entry.amount));
      formData.append(`Entries[${index}].categoryName`, entry.categoryName);

      if (entry.comment) {
        formData.append(`Entries[${index}].comment`, entry.comment);
      }

      if (entry.file) {
        formData.append(`Entries[${index}].file`, entry.file);
      }
    });

    return await AxiosWrapper.post(
      ApiUrl.AddExpenseAdvance(employeeId),
      formData
    );
  }

  public static async updateExpenseAdvance(
    advanceId: string,
    data: UpdateExpenseAdvance
  ) {
    const formData = new FormData();

    formData.append("date", data.date);
    formData.append("type", data.type);
    formData.append("name", data.name);
    formData.append("amount", String(data.amount));
    formData.append("categoryName", data.categoryName);

    if (data.comment) {
      formData.append("comment", data.comment);
    }

    if (data.file) {
      formData.append("file", data.file);
    }

    return await AxiosWrapper.patch(
      ApiUrl.UpdateExpenseAdvance(advanceId),
      formData
    );
  }

  public static async deleteExpenseAdvance(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteExpenseAdvance(id));
  }

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
