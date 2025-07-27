import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  AddExpenseContractorData,
  ExpensesContractorsQueryResponse,
} from "../models/expenses/expenses-contractors";
import type { ExpensesTypesQueryResponse } from "../models/expenses/expenses-types";
import type {
  AddExpenseProductionData,
  DraftExpenseInvoice,
  ExpenseProductionListModel,
  SaveExpenseInvoiceData,
  UpdateExpenseProductionData,
} from "../models/expenses/production/expenses-productions";
import type { ExpensesProductionsDictionary } from "../models/expenses/production/expenses-productions-dictionary";
import type ExpensesProductionsFilter from "../models/expenses/production/expenses-productions-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export interface UploadExpensesProductionsFilesResponse {
  files: DraftExpenseInvoice[];
}

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

  public static async addExpenseProduction(data: AddExpenseProductionData) {
    const formData = new FormData();

    formData.append("farmId", data.farmId);
    formData.append("cycleId", data.cycleId);
    formData.append("expenseContractorId", data.expenseContractorId);
    formData.append("invoiceNumber", data.invoiceNumber);
    formData.append("invoiceTotal", String(data.invoiceTotal));
    formData.append("subTotal", String(data.subTotal));
    formData.append("vatAmount", String(data.vatAmount));
    formData.append("invoiceDate", data.invoiceDate);

    if (data.file) {
      formData.append("file", data.file);
    }

    return await AxiosWrapper.post(ApiUrl.AddExpenseProduction, formData);
  }

  public static async updateExpenseProduction(
    id: string,
    data: UpdateExpenseProductionData
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateExpenseProduction(id), data);
  }

  public static async deleteExpenseProduction(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteExpenseProduction(id));
  }

  public static async uploadInvoices(files: File[]) {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });

    return await AxiosWrapper.post<UploadExpensesProductionsFilesResponse>(
      ApiUrl.UploadExpensesProductions,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
  }

  public static async saveExpenseInvoice(invoiceData: SaveExpenseInvoiceData) {
    return await AxiosWrapper.post(ApiUrl.SaveExpenseInvoiceData, invoiceData);
  }
}
