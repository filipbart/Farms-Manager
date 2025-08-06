import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  AddEmployeePayslipData,
  GetEmployeePayslipsResponse,
  UpdateEmployeePayslip,
} from "../models/employees/employees-payslips";
import type {
  EmployeePayslipsDictionary,
  EmployeePayslipsFilterPaginationModel,
} from "../models/employees/employees-payslips-filters";
import type { FarmPayslipRowModel } from "../models/employees/payslips-farms";
import AxiosWrapper from "../utils/axios/wrapper";

export class EmployeePayslipsService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<EmployeePayslipsDictionary>(ApiUrl.FeedsDict);
  }

  public static async getPayslipFarms() {
    return await AxiosWrapper.get<PaginateModel<FarmPayslipRowModel>>(
      ApiUrl.EmployeePayslipsFarms
    );
  }

  public static async getPayslips(
    filters: EmployeePayslipsFilterPaginationModel
  ) {
    return await AxiosWrapper.get<GetEmployeePayslipsResponse>(
      ApiUrl.EmployeePayslips,
      { ...filters }
    );
  }

  public static async addPayslip(data: AddEmployeePayslipData) {
    return await AxiosWrapper.post(ApiUrl.AddEmployeePayslip, data);
  }

  public static async updatePayslip(id: string, data: UpdateEmployeePayslip) {
    return await AxiosWrapper.patch(ApiUrl.UpdateEmployeePayslip(id), data);
  }

  public static async deletePayslip(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteEmployeePayslip(id));
  }
}
