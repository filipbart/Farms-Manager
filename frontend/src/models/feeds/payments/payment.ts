export enum FeedPaymentStatus {
  Unrealized = "Unrealized",
  Realized = "Realized",
}

export const FeedPaymentStatusLabels: Record<FeedPaymentStatus, string> = {
  [FeedPaymentStatus.Unrealized]: "Niezrealizowany",
  [FeedPaymentStatus.Realized]: "Zrealizowany",
};

export interface FeedPaymentListModel {
  id: string;
  cycleText: string;
  farmName: string;
  filePath: string;
  fileName: string;
  dateCreatedUtc: string;
  status: "Niezrealizowany" | "Zrealizowany";
  comment?: string;
}

export interface MarkPaymentAsCompletedDto {
  comment?: string;
}
