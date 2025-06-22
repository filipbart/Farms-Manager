export default class ApiUrl {
  private static BaseUrl = import.meta.env.PROD
    ? "/api/"
    : import.meta.env.VITE_API_URL || "/api/";
  public static Me = this.BaseUrl + "user/me";
  public static Authenticate = this.BaseUrl + "auth/authenticate";
  public static RefreshToken = this.BaseUrl + "auth/refresh-token";

  public static Farms = this.BaseUrl + "farms";
  public static AddFarm = this.BaseUrl + "farms/add";
  public static DeleteFarm = this.BaseUrl + "farms/delete";
  public static DeleteHenhouse = this.BaseUrl + "farms/henhouse/delete";
}
