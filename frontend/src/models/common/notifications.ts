export enum NotificationType {
  Low = "Low",
  Medium = "Medium",
  High = "High",
}

export interface NotificationInfo {
  count: number;
  priority: NotificationType;
}

export interface NotificationData {
  salesInvoices: NotificationInfo;
  feedDeliveries: NotificationInfo;
  employees: NotificationInfo;
}

export interface NotificationDataQueryResponse {
  data: NotificationData;
}
