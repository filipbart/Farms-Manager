import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  AddEmployeeData,
  AddEmployeeReminderData,
  EmployeeDetailsModel,
  EmployeeListModel,
  UpdateEmployeeData,
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

  public static async getEmployeeDetails(id: string) {
    return await AxiosWrapper.get<EmployeeDetailsModel>(
      ApiUrl.EmployeeDetails(id)
    );
  }

  public static async addEmployee(data: AddEmployeeData) {
    return await AxiosWrapper.post(ApiUrl.AddEmployee, data);
  }

  public static async updateEmployee(id: string, data: UpdateEmployeeData) {
    return await AxiosWrapper.patch(ApiUrl.UpdateEmployee(id), data);
  }

  public static async deleteEmployee(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteEmployee(id));
  }

  public static async uploadEmployeeFiles(id: string, files: File[]) {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });
    return await AxiosWrapper.post(ApiUrl.UploadEmployeeFiles(id), formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  }

  public static async deleteEmployeeFile(id: string, fileId: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteEmployeeFile(id, fileId));
  }

  public static async addEmployeeReminder(
    id: string,
    data: AddEmployeeReminderData
  ) {
    return await AxiosWrapper.post(ApiUrl.AddEmployeeReminder(id), data);
  }

  public static async deleteEmployeeReminder(id: string, reminderId: string) {
    return await AxiosWrapper.delete(
      ApiUrl.DeleteEmployeeReminder(id, reminderId)
    );
  }
}
