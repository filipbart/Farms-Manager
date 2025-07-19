import type { FeedInvoiceData } from "./feed-invoice";

export interface DraftFeedInvoice {
  draftId: string;
  filePath: string;
  fileUrl: string;
  extractedFields: FeedInvoiceData;
}
