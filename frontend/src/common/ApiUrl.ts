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

  public static Slaughterhouses = this.BaseUrl + "slaughterhouses";
  public static AddSlaughterhouse = this.Slaughterhouses + "/add";
  public static DeleteSlaughterhouse = this.Slaughterhouses + "/delete";

  public static Insertions = this.BaseUrl + "insertions";
  public static UpdateInsertion = this.Insertions + "/update";
  public static InsertionsDict = this.Insertions + "/dictionary";
  public static LatestCycle = "/latest-cycle";
  public static InsertionAvailableHenhouses =
    this.Insertions + "/available-henhouses";
  public static InsertionSendToIrz = this.Insertions + "/send-to-irz";

  public static Settings = this.BaseUrl + "settings";
  public static IrzPlusCredentials =
    this.Settings + "/save-irzplus-credentials";

  public static Sales = this.BaseUrl + "sales";
  public static SalesDict = this.Sales + "/dictionary";
  public static UpdateSale = this.Sales + "/update";
  public static SaleSendToIrz = this.Sales + "/send-to-irz";
  public static SaleExportFile = this.Sales + "/export";

  public static SalesSettings = this.BaseUrl + "sales-settings";
  public static AddSaleFieldExtra = this.SalesSettings + "/add";
  public static DeleteSaleFieldExtra = this.SalesSettings + "/delete";

  public static Feeds = this.BaseUrl + "feeds";
  public static FeedsDict = this.Feeds + "/dictionary";
  public static FeedsNames = this.Feeds + "/names";
  public static AddFeedName = this.FeedsNames + "/add";
  public static FeedsPrices = this.Feeds + "/prices";
  public static DeleteFeedName = this.FeedsNames + "/delete";
  public static AddFeedPrice = this.Feeds + "/add-price";
}
