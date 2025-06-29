export default class ApiUrl {
  private static BaseUrl = import.meta.env.PROD
    ? "/api/"
    : import.meta.env.VITE_API_URL || "/api/";

  public static User = this.BaseUrl + "user";
  public static Me = this.User + "/me";
  public static UserDetails = this.User + "/details";
  public static Authenticate = this.BaseUrl + "auth/authenticate";
  public static RefreshToken = this.BaseUrl + "auth/refresh-token";

  public static Farms = this.BaseUrl + "farms";
  public static AddFarm = this.Farms + "/add";
  public static DeleteFarm = this.Farms + "/delete";
  public static DeleteHenhouse = this.Farms + "/henhouse/delete";

  public static Hatcheries = this.BaseUrl + "hatcheries";
  public static AddHatchery = this.Hatcheries + "/add";
  public static DeleteHatchery = this.Hatcheries + "/delete";

  public static Insertions = this.BaseUrl + "insertions";
  public static UpdateInsertion = this.Insertions + "/update";
  public static InsertionsDict = this.Insertions + "/dictionary";
  public static LatestCycle = "/latest-cycle";
  public static InsertionAvailableHenhouses =
    this.Insertions + "/available-henhouses";
  public static SendToIrz = this.Insertions + "/send-to-irz";

  public static Settings = this.BaseUrl + "settings";
  public static IrzPlusCredentials =
    this.Settings + "/save-irzplus-credentials";
}
