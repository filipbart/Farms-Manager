import ApiUrl from "../common/ApiUrl";
import type AuthUserResponse from "../models/auth/auth-user-response";
import type RefreshTokenResponse from "../models/auth/refresh-token-response";
import AxiosWrapper from "../utils/axios/wrapper";

export class AuthService {
  public static async login(login: string, password: string) {
    return await AxiosWrapper.post<AuthUserResponse>(ApiUrl.Authenticate, {
      login,
      password,
    });
  }

  public static async logout() {
    return await AxiosWrapper.post(ApiUrl.Logout);
  }

  public static async refreshToken() {
    return await AxiosWrapper.post<RefreshTokenResponse>(ApiUrl.RefreshToken);
  }
}
