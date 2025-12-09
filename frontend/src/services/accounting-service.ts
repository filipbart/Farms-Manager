import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  KSeFInvoiceDetails,
  KSeFInvoiceListModel,
} from "../models/accounting/ksef-invoice";
import type { KSeFInvoicesFilters } from "../models/accounting/ksef-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export interface UploadKSeFInvoiceResponse {
  invoiceId: string;
  message: string;
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
   * Upload manualnej faktury (poza KSeF)
   */
  public static async uploadManualInvoice(
    files: File[],
    metadata: {
      invoiceType: string;
      invoiceNumber?: string;
      invoiceDate?: string;
    }
  ) {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });
    formData.append("invoiceType", metadata.invoiceType);
    if (metadata.invoiceNumber) {
      formData.append("invoiceNumber", metadata.invoiceNumber);
    }
    if (metadata.invoiceDate) {
      formData.append("invoiceDate", metadata.invoiceDate);
    }

    return await AxiosWrapper.post<UploadKSeFInvoiceResponse>(
      ApiUrl.AccountingUploadInvoice,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
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
      comment?: string;
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
