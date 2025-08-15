import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  AddUserData,
  UpdateUserData,
  UserDetailsModel,
  UserListModel,
} from "../models/users/users";
import type { UsersFilterPaginationModel } from "../models/users/users-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export class UsersService {
  public static async getUsers(filters: UsersFilterPaginationModel) {
    return await AxiosWrapper.get<PaginateModel<UserListModel>>(ApiUrl.Users, {
      ...filters,
    });
  }

  public static async getUserDetails(id: string) {
    return await AxiosWrapper.get<UserDetailsModel>(ApiUrl.UsersDetails(id));
  }

  public static async addUser(data: AddUserData) {
    return await AxiosWrapper.post(ApiUrl.AddUser, data);
  }

  public static async updateUser(id: string, data: UpdateUserData) {
    return await AxiosWrapper.patch(ApiUrl.UpdateUser(id), data);
  }

  public static async deleteUser(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteUser(id));
  }
}
