import type { NotificationPriority } from "../common/notifications";

export type NotificationType =
  | "SaleInvoice"
  | "FeedInvoice"
  | "EmployeeContract"
  | "EmployeeReminder";

export interface DashboardNotificationItem {
  description: string;
  dueDate: string;
  priority: NotificationPriority;
  type: NotificationType;
  sourceId: string;
}

export interface FarmDictModel {
  id: string;
  name: string;
}

export interface CycleDictModel {
  identifier: number;
  year: number;
}

export interface DashboardDictionary {
  farms: FarmDictModel[];
  cycles: CycleDictModel[];
}

export interface DashboardFilters {
  farmId?: string;
  cycle?: string;
  dateSince?: string;
  dateTo?: string;
  dateCategory?: string;
}

// --- Odpowiedzi z poszczególnych endpointów ---

// /stats
export interface DashboardStatsResponse {
  stats: DashboardStats;
  chickenHousesStatus: DashboardChickenHousesStatus;
}

// /notifications
export type DashboardNotificationsResponse = DashboardNotificationItem[];

// /fcr-chart
export interface DashboardFcrChart {
  series: ChartSeries[];
}

// /eww-chart
export interface DashboardEwwChart {
  series: ChartSeries[];
}

// /flock-loss-chart
export interface DashboardFlockLossChart {
  series: ChartSeries[];
}

// /gas-consumption-chart
export interface DashboardGasConsumptionChart {
  series: ChartSeries[];
}

// /expenses-pie-chart
export interface DashboardExpensesPieChart {
  data: ExpensesPieChartDataPoint[];
}

// --- Współdzielone modele ---

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

export interface ChartSeries {
  farmId: string;
  farmName: string;
  data: ChartDataPoint[];
}

export interface ChartDataPoint {
  x: string;
  y: number | null;
}

export interface ExpensesPieChartDataPoint {
  id: string;
  label: string;
  value: number;
}