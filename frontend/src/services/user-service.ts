import ApiUrl from "../common/ApiUrl";
import type UserModel from "../common/interfaces/user-model";
import AxiosWrapper from "../utils/axios/wrapper";

export class UserService {
  public static async getUserAsync() {
    return await AxiosWrapper.get<UserModel>(ApiUrl.Me);
  }
}
