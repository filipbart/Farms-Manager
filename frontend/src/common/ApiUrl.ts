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

  public static Hatcheries = this.BaseUrl + "hatcheries";
  public static AddHatchery = this.BaseUrl + "hatcheries/add";
  public static DeleteHatchery = this.BaseUrl + "hatcheries/delete";

  public static Insertions = this.BaseUrl + "insertions";
  public static InsertionsDict = this.BaseUrl + "insertions/dictionary";
  public static LatestCycle = "/latest-cycle";
}
