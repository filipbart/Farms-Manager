import ApiUrl from "../common/ApiUrl";
import AxiosWrapper from "../utils/axios/wrapper";

export class SettingsService {
  public static async saveIrzPlusCredentials(data: {
    login: string;
    password: string;
  }) {
    return await AxiosWrapper.post(ApiUrl.IrzPlusCredentials, data);
  }
}
