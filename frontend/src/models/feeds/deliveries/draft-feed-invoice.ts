import type { FeedInvoiceData } from "./feed-invoice";

export interface DraftFeedInvoice {
  draftId: string;
  fileUrl: string;
  extractedFields: FeedInvoiceData;
}
