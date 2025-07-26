import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  AddExpenseContractorData,
  ExpensesContractorsQueryResponse,
} from "../models/expenses/expenses-contractors";
import type { ExpensesTypesQueryResponse } from "../models/expenses/expenses-types";
import type { ExpenseProductionListModel } from "../models/expenses/production/expenses-productions";
import type { ExpensesProductionsDictionary } from "../models/expenses/production/expenses-productions-dictionary";
import type ExpensesProductionsFilter from "../models/expenses/production/expenses-productions-filters";
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

  public static async getDictionaries() {
    return await AxiosWrapper.get<ExpensesProductionsDictionary>(
      ApiUrl.ExpensesProductionsDictionary
    );
  }

  public static async getExpensesProductions(
    filters: ExpensesProductionsFilter
  ) {
    return await AxiosWrapper.get<PaginateModel<ExpenseProductionListModel>>(
      ApiUrl.ExpensesProductions,
      { ...filters }
    );
  }

  public static async deleteExpenseProduction(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteExpenseProduction(id));
  }
}
