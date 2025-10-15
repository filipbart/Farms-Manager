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
