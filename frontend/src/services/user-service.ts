import ApiUrl from "../common/ApiUrl";
import type UserModel from "../common/interfaces/user-model";
import type { NotificationDataQueryResponse } from "../models/common/notifications";
import type { UpdateMyData } from "../models/user/user-details";
import AxiosWrapper from "../utils/axios/wrapper";

export class UserService {
  public static async getUser() {
    return await AxiosWrapper.get<UserModel>(ApiUrl.Me);
  }

  public static async getDetails() {
    return await AxiosWrapper.get<UserModel>(ApiUrl.UserDetails);
  }

  public static async getNotifications() {
    return await AxiosWrapper.get<NotificationDataQueryResponse>(
      ApiUrl.GetNotifications
    );
  }

  public static async updateData(data: UpdateMyData) {
    return await AxiosWrapper.patch(ApiUrl.Me, data);
  }
}
