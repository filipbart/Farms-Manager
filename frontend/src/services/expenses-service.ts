import ApiUrl from "../common/ApiUrl";
import type { ExpensesTypesQueryResponse } from "../models/expenses/expenses-types";
import AxiosWrapper from "../utils/axios/wrapper";

export class ExpensesService {
  public static async getExpensesTypes() {
    return await AxiosWrapper.get<ExpensesTypesQueryResponse>(
      ApiUrl.ExpensesTypes
    );
  }

  public static async addExpensesType(types: string[]) {
    return await AxiosWrapper.post(ApiUrl.AddExpensesType, {
      types,
    });
  }

  public static async deleteExpensesType(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteExpensesType(id));
  }
}
