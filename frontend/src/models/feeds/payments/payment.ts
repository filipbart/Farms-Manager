export enum FeedPaymentStatus {
  Unrealized = "Unrealized",
  Realized = "Realized",
}

export const FeedPaymentStatusLabels: Record<FeedPaymentStatus, string> = {
  [FeedPaymentStatus.Unrealized]: "Niezrealizowany",
  [FeedPaymentStatus.Realized]: "Zrealizowany",
};

import type { AuditFields } from "../../../common/interfaces/audit-fields";

export interface FeedPaymentListModel extends AuditFields {
  id: string;
  cycleText: string;
  farmName: string;
  filePath: string;
  fileName: string;
  dateCreatedUtc: string;
  status: FeedPaymentStatus;
  comment?: string;
}

export interface MarkPaymentAsCompletedDto {
  comment?: string;
}
