import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  AddEmployeeData,
  EmployeeListModel,
} from "../models/employees/employees";
import type {
  EmployeesDictionary,
  EmployeesFilterPaginationModel,
} from "../models/employees/employees-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export class EmployeesService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<EmployeesDictionary>(
      ApiUrl.EmployeesDictionary
    );
  }

  public static async getEmployees(filters: EmployeesFilterPaginationModel) {
    return await AxiosWrapper.get<PaginateModel<EmployeeListModel>>(
      ApiUrl.Employees,
      { ...filters }
    );
  }

  public static async addEmployee(data: AddEmployeeData) {
    return await AxiosWrapper.post(ApiUrl.AddEmployee, data);
  }

  public static async deleteEmployee(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteEmployee(id));
  }
}
