import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  KSeFInvoiceDetails,
  KSeFInvoiceListModel,
} from "../models/accounting/ksef-invoice";
import type { KSeFInvoicesFilters } from "../models/accounting/ksef-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AccountingInvoiceExtractedData {
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  sellerName: string;
  sellerNip: string;
  sellerAddress: string;
  buyerName: string;
  buyerNip: string;
  buyerAddress: string;
  grossAmount: number | null;
  netAmount: number | null;
  vatAmount: number | null;
  bankAccountNumber: string;
  invoiceType: string;
}

export interface DraftAccountingInvoice {
  draftId: string;
  fileUrl: string;
  filePath: string;
  extractedFields: AccountingInvoiceExtractedData;
}

export interface UploadAccountingInvoicesResponse {
  files: DraftAccountingInvoice[];
}

export interface SaveAccountingInvoiceData {
  draftId: string;
  filePath: string;
  invoiceNumber: string;
  invoiceDate: string;
  dueDate?: string;
  sellerName: string;
  sellerNip: string;
  buyerName: string;
  buyerNip: string;
  grossAmount: number;
  netAmount: number;
  vatAmount: number;
  invoiceType: string;
  comment?: string;
}

export class AccountingService {
  /**
   * Pobiera listę faktur KSeF z paginacją i filtrami
   */
  public static async getKSeFInvoices(filters: KSeFInvoicesFilters) {
    return await AxiosWrapper.get<PaginateModel<KSeFInvoiceListModel>>(
      ApiUrl.AccountingInvoices,
      { ...filters }
    );
  }

  /**
   * Pobiera szczegóły faktury KSeF
   */
  public static async getKSeFInvoiceDetails(invoiceId: string) {
    return await AxiosWrapper.get<KSeFInvoiceDetails>(
      ApiUrl.AccountingInvoiceDetails(invoiceId)
    );
  }

  /**
   * Upload faktur z zaczytywaniem danych przez AI
   */
  public static async uploadInvoices(
    files: File[],
    invoiceType: string,
    signal?: AbortSignal
  ) {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });
    formData.append("invoiceType", invoiceType);

    return await AxiosWrapper.post<UploadAccountingInvoicesResponse>(
      ApiUrl.AccountingUploadInvoice,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        signal,
      }
    );
  }

  /**
   * Zapisuje fakturę po zaczytaniu przez AI
   */
  public static async saveInvoice(data: SaveAccountingInvoiceData) {
    return await AxiosWrapper.post<string>(ApiUrl.AccountingSaveInvoice, data);
  }

  /**
   * Aktualizuje fakturę KSeF
   */
  public static async updateInvoice(
    invoiceId: string,
    data: {
      status?: string;
      paymentStatus?: string;
      moduleType?: string;
      vatDeductionType?: string;
      comment?: string;
      farmId?: string | null;
      cycleId?: string | null;
      assignedUserId?: string | null;
      relatedInvoiceNumber?: string;
    }
  ) {
    return await AxiosWrapper.patch(
      ApiUrl.AccountingUpdateInvoice(invoiceId),
      data
    );
  }

  /**
   * Usuwa fakturę KSeF
   */
  public static async deleteInvoice(invoiceId: string) {
    return await AxiosWrapper.delete(ApiUrl.AccountingDeleteInvoice(invoiceId));
  }

  /**
   * Synchronizacja z KSeF
   */
  public static async syncWithKSeF() {
    return await AxiosWrapper.post(ApiUrl.AccountingSyncKSeF, {});
  }
}
