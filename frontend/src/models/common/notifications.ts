export enum NotificationPriority {
  Low = "Low",
  Medium = "Medium",
  High = "High",
}

export interface NotificationInfo {
  count: number;
  priority: NotificationPriority;
}

export interface NotificationData {
  salesInvoices: NotificationInfo;
  feedDeliveries: NotificationInfo;
  employees: NotificationInfo;
  accountingInvoices: NotificationInfo;
}

export interface NotificationDataQueryResponse {
  data: NotificationData;
}
