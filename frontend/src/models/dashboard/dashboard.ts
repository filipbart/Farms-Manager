import type { NotificationPriority } from "../common/notifications";

export type NotificationType =
  | "SaleInvoice"
  | "FeedInvoice"
  | "EmployeeContract"
  | "EmployeeReminder";

export interface DashboardStats {
  revenue: number;
  expenses: number;
  income: number;
  vatFromExpenses: number;
  incomePerKg: number;
  incomePerSqm: number;
  avgFeedPrice: number;
}

export interface DashboardHenhouseStatus {
  name: string;
  chickenCount: number;
}

export interface DashboardFarmStatus {
  name: string;
  henhousesCount: number;
  chickenCount: number;
  henhouses: DashboardHenhouseStatus[];
}

export interface DashboardChickenHousesStatus {
  farms: DashboardFarmStatus[];
  totalHenhousesCount: number;
  totalChickenCount: number;
}

export interface ChartDataPoint {
  x: string;
  y: number | null;
}

export interface ChartSeries {
  farmId: string;
  farmName: string;
  data: ChartDataPoint[];
}

export interface DashboardFcrChart {
  series: ChartSeries[];
}

export interface DashboardGasConsumptionChart {
  series: ChartSeries[];
}

export interface DashboardEwwChart {
  series: ChartSeries[];
}

export interface DashboardFlockLossChart {
  series: ChartSeries[];
}

export interface ExpensesPieChartDataPoint {
  id: string;
  label: string;
  value: number;
}

export interface DashboardExpensesPieChart {
  data: ExpensesPieChartDataPoint[];
}

export interface DashboardNotificationItem {
  description: string;
  dueDate: string;
  priority: NotificationPriority;
  type: NotificationType;
  sourceId: string;
}

export interface GetDashboardDataQueryResponse {
  stats: DashboardStats;
  fcrChart: DashboardFcrChart;
  gasConsumptionChart: DashboardGasConsumptionChart;
  ewwChart: DashboardEwwChart;
  flockLossChart: DashboardFlockLossChart;
  chickenHousesStatus: DashboardChickenHousesStatus;
  expensesPieChart: DashboardExpensesPieChart;
  notifications: DashboardNotificationItem[];
}
