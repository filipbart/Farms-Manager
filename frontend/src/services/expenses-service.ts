import ApiUrl from "../common/ApiUrl";
import type {
  AddExpenseContractorData,
  ExpensesContractorsQueryResponse,
} from "../models/expenses/expenses-contractors";
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

  public static async getExpensesContractors() {
    return await AxiosWrapper.get<ExpensesContractorsQueryResponse>(
      ApiUrl.ExpensesContractors
    );
  }

  public static async deleteExpenseContractor(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteExpenseContractor(id));
  }

  public static async addExpenseContractor(data: AddExpenseContractorData) {
    return await AxiosWrapper.post(ApiUrl.AddExpensesContractor, data);
  }

  public static async updateExpenseContractor(
    id: string,
    data: AddExpenseContractorData
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateExpenseContractor(id), data);
  }
}
