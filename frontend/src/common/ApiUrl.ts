export default class ApiUrl {
  private static BaseUrl = import.meta.env.PROD
    ? "/api/"
    : import.meta.env.VITE_API_URL || "/api/";

  public static User = this.BaseUrl + "user";
  public static Me = this.User + "/me";
  public static UserDetails = this.User + "/details";
  public static GetNotifications = this.User + "/notifications";
  public static Authenticate = this.BaseUrl + "auth/authenticate";
  public static RefreshToken = this.BaseUrl + "auth/refresh-token";
  public static Logout = this.BaseUrl + "auth/logout";

  public static Files = this.BaseUrl + "files";
  public static GetFile = `${this.Files}/file`;

  public static Farms = this.BaseUrl + "farms";
  public static UpdateFarmCycle = this.Farms + "/update-cycle";
  public static AddFarm = this.Farms + "/add";
  public static DeleteFarm = this.Farms + "/delete";
  public static DeleteHenhouse = this.Farms + "/henhouse/delete";

  public static Hatcheries = this.BaseUrl + "hatcheries";
  public static AddHatchery = this.Hatcheries + "/add";
  public static DeleteHatchery = this.Hatcheries + "/delete";
  public static HatcheriesPrices = this.Hatcheries + "/prices";
  public static HatcheriesPricesDictionary =
    this.HatcheriesPrices + "/dictionary";
  public static AddHatcheryPrice = this.HatcheriesPrices + "/add";
  public static UpdateHatcheryPrice = (id: string) =>
    this.HatcheriesPrices + "/update/" + id;
  public static DeleteHatcheryPrice = (id: string) =>
    this.HatcheriesPrices + "/delete/" + id;
  public static HatcheriesNotes = this.Hatcheries + "/notes";
  public static AddHatcheryNote = this.HatcheriesNotes + "/add";
  public static UpdateHatcheryNote = (id: string) =>
    this.HatcheriesNotes + "/update/" + id;
  public static DeleteHatcheryNote = (id: string) =>
    this.HatcheriesNotes + "/delete/" + id;

  public static Slaughterhouses = this.BaseUrl + "slaughterhouses";
  public static AddSlaughterhouse = this.Slaughterhouses + "/add";
  public static DeleteSlaughterhouse = this.Slaughterhouses + "/delete";

  public static Insertions = this.BaseUrl + "insertions";
  public static UpdateInsertion = this.Insertions + "/update";
  public static DeleteInsertion = (id: string) =>
    this.Insertions + "/delete/" + id;
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
  public static DeleteSale = (id: string) => this.Sales + "/delete/" + id;
  public static SaleSendToIrz = this.Sales + "/send-to-irz";
  public static SaleExportFile = this.Sales + "/export";
  public static SaleDownloadZip = this.Sales + "/download";
  public static SalesInvoices = this.Sales + "/invoices";
  public static UploadSalesInvoices = this.SalesInvoices + "/upload";
  public static SaveSalesInvoicesData = this.SalesInvoices + "/save-invoice";
  public static BookSalesInvoicesPayment = this.SalesInvoices + "/book-payment";
  public static UpdateSaleInvoice = (id: string) =>
    this.SalesInvoices + "/update/" + id;
  public static DeleteSaveInvoice = (id: string) =>
    this.SalesInvoices + "/delete/" + id;

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
  public static UpdateFeedPrice = this.Feeds + "/update-price";
  public static DeleteFeedPrice = this.Feeds + "/delete-price";
  public static UploadDeliveries = this.Feeds + "/upload-deliveries";
  public static SaveInvoiceData = this.Feeds + "/save-invoice";
  public static FeedsDeliveries = this.Feeds + "/deliveries";
  public static DownloadFeedDeliveryFile = this.Feeds + "/download-file";
  public static DownloadPaymentFile = this.Feeds + "/payment-file";
  public static DeleteFeedDelivery = this.Feeds + "/delete-delivery";
  public static UpdateFeedDelivery = this.Feeds + "/update-delivery";
  public static FeedsPayments = this.Feeds + "/payments";
  public static DeleteFeedPayment = this.Feeds + "/delete-payment";
  public static AddFeedCorrection = this.Feeds + "/add-correction";
  public static DeleteFeedCorrection = this.Feeds + "/delete-correction";
  public static UpdateFeedCorrection = this.Feeds + "/update-correction";

  public static Expenses = this.BaseUrl + "expenses";
  public static ExpensesTypes = this.Expenses + "/types";
  public static AddExpensesType = this.Expenses + "/add-type";
  public static DeleteExpensesType = (id: string) =>
    this.Expenses + "/delete-type/" + id;
  public static ExpensesContractors = this.Expenses + "/contractors";
  public static AddExpensesContractor = this.Expenses + "/add-contractor";
  public static DeleteExpenseContractor = (id: string) =>
    this.Expenses + "/delete-contractor/" + id;
  public static UpdateExpenseContractor = (id: string) =>
    this.Expenses + "/update-contractor/" + id;
  public static ExpensesProductionsDictionary =
    this.Expenses + "/productions/dictionary";
  public static ExpensesProductions = this.Expenses + "/productions";
  public static AddExpenseProduction = this.ExpensesProductions + "/add";
  public static UpdateExpenseProduction = (id: string) =>
    this.ExpensesProductions + "/update/" + id;
  public static DeleteExpenseProduction = (id: string) =>
    this.ExpensesProductions + "/delete/" + id;
  public static UploadExpensesProductions =
    this.ExpensesProductions + "/upload";
  public static SaveExpenseInvoiceData =
    this.ExpensesProductions + "/save-invoice";

  public static ExpensesAdvances = this.Expenses + "/advances";
  public static ExpensesAdvancesCategories =
    this.ExpensesAdvances + "/categories";
  public static AddAdvanceCategory = this.ExpensesAdvancesCategories + "/add";
  public static DeleteAdvanceCategory = (id: string) =>
    this.ExpensesAdvancesCategories + "/delete/" + id;

  public static ProductionData = this.BaseUrl + "production-data";
  public static ProductionDataDictionary = this.ProductionData + "/dictionary";
  public static CalculateRemainingFeedValue =
    this.ProductionData + "/calculate-value";

  public static ProductionDataFailures = this.ProductionData + "/failures";
  public static AddProductionDataFailure = this.ProductionDataFailures + "/add";
  public static UpdateProductionDataFailure = (id: string) =>
    this.ProductionDataFailures + "/update/" + id;
  public static DeleteProductionDataFailure = (id: string) =>
    this.ProductionDataFailures + "/delete/" + id;

  public static ProductionDataRemainingFeed =
    this.ProductionData + "/remaining-feed";
  public static AddProductionDataRemainingFeed =
    this.ProductionDataRemainingFeed + "/add";
  public static UpdateProductionDataRemainingFeed = (id: string) =>
    this.ProductionDataRemainingFeed + "/update/" + id;
  public static DeleteProductionDataRemainingFeed = (id: string) =>
    this.ProductionDataRemainingFeed + "/delete/" + id;

  public static ProductionDataTransferFeed =
    this.ProductionData + "/transfer-feed";
  public static AddProductionDataTransferFeed =
    this.ProductionDataTransferFeed + "/add";
  public static UpdateProductionDataTransferFeed = (id: string) =>
    this.ProductionDataTransferFeed + "/update/" + id;
  public static DeleteProductionDataTransferFeed = (id: string) =>
    this.ProductionDataTransferFeed + "/delete/" + id;

  public static ProductionDataWeighings = this.ProductionData + "/weighings";
  public static ProductionDataWeighingsDictionary =
    this.ProductionDataWeighings + "/dictionary";
  public static AddProductionDataWeighing =
    this.ProductionDataWeighings + "/add";
  public static UpdateProductionDataWeighing = (id: string) =>
    this.ProductionDataWeighings + "/update/" + id;
  public static DeleteProductionDataWeighing = (id: string) =>
    this.ProductionDataWeighings + "/delete/" + id;

  public static WeightStandards = this.ProductionDataWeighings + "/standards";
  public static AddWeightStandards = this.WeightStandards + "/add";
  public static DeleteWeightStandard = (id: string) =>
    this.WeightStandards + "/delete/" + id;
  public static GetHatcheryForWeighing =
    this.ProductionDataWeighings + "/get-hatchery";

  public static Gas = this.BaseUrl + "gas";
  public static GasDeliveriesDictionary = this.Gas + "/dictionary";
  public static GasContractors = this.Gas + "/contractors";
  public static GasDeliveries = this.Gas + "/deliveries";
  public static AddGasDelivery = this.GasDeliveries + "/add";
  public static UpdateGasDelivery = (id: string) =>
    this.GasDeliveries + "/update/" + id;
  public static DeleteGasDelivery = (id: string) =>
    this.GasDeliveries + "/delete/" + id;
  public static UploadGasInvoices = this.GasDeliveries + "/upload";
  public static SaveGasInvoiceData = this.GasDeliveries + "/save-invoice";
  public static GasConsumptions = this.Gas + "/consumptions";
  public static GetGasConsumptionsDictionary =
    this.GasConsumptions + "/dictionary";
  public static AddGasConsumption = this.GasConsumptions + "/add";
  public static UpdateGasConsumption = (id: string) =>
    this.GasConsumptions + "/update/" + id;
  public static DeleteGasConsumption = (id: string) =>
    this.GasConsumptions + "/delete/" + id;
  public static CalculateGasCost = this.GasConsumptions + "/calculate-cost";

  public static Employees = this.BaseUrl + "employees";
  public static EmployeesDictionary = this.Employees + "/dictionary";
  public static EmployeeDetails = (id: string) => this.Employees + "/" + id;
  public static AddEmployee = this.Employees + "/add";
  public static UpdateEmployee = (id: string) =>
    this.EmployeeDetails(id) + "/update";
  public static DeleteEmployee = (id: string) =>
    this.EmployeeDetails(id) + "/delete";
  public static UploadEmployeeFiles = (id: string) =>
    this.EmployeeDetails(id) + "/upload-files";
  public static DeleteEmployeeFile = (id: string, fileId: string) =>
    this.EmployeeDetails(id) + "/delete-file/" + fileId;
  public static AddEmployeeReminder = (id: string) =>
    this.EmployeeDetails(id) + "/add-reminder";
  public static DeleteEmployeeReminder = (id: string, reminderId: string) =>
    this.EmployeeDetails(id) + "/delete-reminder/" + reminderId;

  public static EmployeePayslips = this.BaseUrl + "employees-payslips";
  public static EmployeePayslipsDictionary =
    this.EmployeePayslips + "/dictionary";
  public static EmployeePayslipsFarms = this.EmployeePayslips + "/farms";
  public static AddEmployeePayslip = this.EmployeePayslips + "/add";
  public static UpdateEmployeePayslip = (id: string) =>
    this.EmployeePayslips + "/update/" + id;
  public static DeleteEmployeePayslip = (id: string) =>
    this.EmployeePayslips + "/delete/" + id;
}
