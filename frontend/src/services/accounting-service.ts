import axios from "axios";
import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type {
  KSeFInvoiceDetails,
  KSeFInvoiceListModel,
  LinkableInvoice,
  LinkInvoicesRequest,
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
  paymentStatus?: string;
  paymentDate?: string;
  farmId?: string;
  cycleId?: string;
  moduleType?: string;

  // Module-specific fields
  feedContractorId?: string;
  gasContractorId?: string;
  expenseContractorId?: string;
  slaughterhouseId?: string;
  henhouseId?: string;
  henhouseName?: string;

  // Feed module specific fields
  feedItemName?: string;
  feedQuantity?: number;
  feedUnitPrice?: number;

  // Gas module specific fields
  gasQuantity?: number;
  gasUnitPrice?: number;

  // Flags for new entities created during upload
  isNewFeedContractor?: boolean;
  isNewGasContractor?: boolean;
  isNewExpenseContractor?: boolean;
  isNewSlaughterhouse?: boolean;
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

export interface UploadXmlInvoicesResponse {
  importedCount: number;
  skippedCount: number;
  errors: string[];
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
  paymentStatus?: string;
  paymentDate?: string;
  grossAmount: number;
  netAmount: number;
  vatAmount: number;
  invoiceType: string;
  documentType?: string;
  status?: string;
  vatDeductionType?: string;
  moduleType: string;
  comment?: string;
  assignedUserId?: string;
  relatedInvoiceNumber?: string;
  // Module-specific data
  feedData?: SaveFeedInvoiceData;
  gasData?: SaveGasDeliveryData;
  expenseData?: SaveExpenseProductionData;
  saleData?: SaveSaleInvoiceData;
}

export interface SaveFeedInvoiceData {
  farmId: string;
  cycleId: string;
  henhouseId: string;
  bankAccountNumber?: string;
  vendorName?: string;
  itemName: string;
  quantity: number;
  unitPrice: number;
}

export interface SaveGasDeliveryData {
  farmId: string;
  cycleId?: string;
  contractorId?: string;
  unitPrice: number;
  quantity: number;
}

export interface SaveExpenseProductionData {
  farmId: string;
  cycleId: string;
  expenseContractorId?: string;
  expenseTypeId: string;
  contractorName?: string;
  contractorNip?: string;
}

export interface SaveSaleInvoiceData {
  farmId: string;
  cycleId: string;
  slaughterhouseId?: string;
}

export class AccountingService {
  /**
   * Pobiera listę faktur KSeF z paginacją i filtrami
   */
  public static async getKSeFInvoices(filters: KSeFInvoicesFilters) {
    const { paymentStatus, ...restFilters } = filters;

    // Usuń puste wartości z parametrów
    const cleanedParams: Record<string, any> = {};

    Object.entries(restFilters).forEach(([key, value]) => {
      // Pomiń puste stringi, undefined, null, puste tablice
      if (
        value !== undefined &&
        value !== null &&
        value !== "" &&
        !(Array.isArray(value) && value.length === 0)
      ) {
        cleanedParams[key] = value;
      }
    });

    // Dodaj paymentStatuses tylko jeśli nie jest puste
    if (paymentStatus?.length) {
      cleanedParams.paymentStatuses = paymentStatus;
    }

    return await AxiosWrapper.get<PaginateModel<KSeFInvoiceListModel>>(
      ApiUrl.AccountingInvoices,
      cleanedParams,
    );
  }

  /**
   * Pobiera szczegóły faktury KSeF
   */
  public static async getKSeFInvoiceDetails(invoiceId: string) {
    return await AxiosWrapper.get<KSeFInvoiceDetails>(
      ApiUrl.AccountingInvoiceDetails(invoiceId),
    );
  }

  /**
   * Upload faktur z zaczytywaniem danych przez AI
   */
  public static async uploadInvoices(
    files: File[],
    invoiceType: string,
    paymentStatus: string,
    moduleType?: string,
    paymentDate?: string | null,
    signal?: AbortSignal,
  ) {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });
    formData.append("invoiceType", invoiceType);
    formData.append("paymentStatus", paymentStatus);
    if (moduleType) {
      formData.append("moduleType", moduleType);
    }
    if (paymentDate) {
      formData.append("paymentDate", paymentDate);
    }

    return await AxiosWrapper.post<UploadAccountingInvoicesResponse>(
      ApiUrl.AccountingUploadInvoice,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        signal,
      },
    );
  }

  /**
   * Upload faktur KSeF z plików XML (bezpośredni import, bez AI)
   */
  public static async uploadXmlInvoices(
    files: File[],
    invoiceType: string,
    paymentStatus: string,
    paymentDate?: string | null,
    signal?: AbortSignal,
  ) {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });
    formData.append("invoiceType", invoiceType);
    formData.append("paymentStatus", paymentStatus);
    if (paymentDate) {
      formData.append("paymentDate", paymentDate);
    }

    return await AxiosWrapper.post<UploadXmlInvoicesResponse>(
      ApiUrl.AccountingUploadXmlInvoice,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        signal,
      },
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
      paymentDate?: string | null;
      dueDate?: string | null;
      moduleType?: string;
      vatDeductionType?: string;
      comment?: string;
      farmId?: string | null;
      cycleId?: string | null;
      assignedUserId?: string | null;
      relatedInvoiceNumber?: string;
      // Additional fields for non-KSeF invoices
      invoiceNumber?: string;
      invoiceDate?: string;
      sellerName?: string;
      sellerNip?: string;
      buyerName?: string;
      buyerNip?: string;
      grossAmount?: number;
      netAmount?: number;
      vatAmount?: number;
      documentType?: string;
      // Gas module fields
      gasQuantity?: number;
      gasUnitPrice?: number;
      // Feed module fields
      henhouseId?: string;
    },
  ) {
    return await AxiosWrapper.patch(
      ApiUrl.AccountingUpdateInvoice(invoiceId),
      data,
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

  /**
   * Pobiera listę faktur możliwych do powiązania
   */
  public static async getLinkableInvoices(
    invoiceId: string,
    searchPhrase?: string,
    limit: number = 20,
  ) {
    return await AxiosWrapper.get<LinkableInvoice[]>(
      ApiUrl.AccountingLinkableInvoices(invoiceId),
      { searchPhrase, limit },
    );
  }

  /**
   * Tworzy powiązania między fakturami
   */
  public static async linkInvoices(data: LinkInvoicesRequest) {
    return await AxiosWrapper.post(ApiUrl.AccountingLinkInvoices, data);
  }

  /**
   * Akceptuje brak powiązania dla faktury
   */
  public static async acceptNoLinking(invoiceId: string) {
    return await AxiosWrapper.post(
      ApiUrl.AccountingAcceptNoLinking(invoiceId),
      {},
    );
  }

  /**
   * Odkłada przypomnienie o powiązaniu
   */
  public static async postponeLinkingReminder(
    invoiceId: string,
    days: number = 3,
  ) {
    return await AxiosWrapper.post(
      `${ApiUrl.AccountingPostponeLinking(invoiceId)}?days=${days}`,
      {},
    );
  }

  /**
   * Tworzy encję w module na podstawie faktury KSeF
   */
  public static async createModuleEntity(
    invoiceId: string,
    data: CreateModuleEntityRequest,
  ) {
    return await AxiosWrapper.post<string | null>(
      ApiUrl.AccountingCreateModuleEntity(invoiceId),
      data,
    );
  }

  /**
   * Akceptuje fakturę i tworzy encję w odpowiednim module.
   * Zmienia status faktury na "Accepted" i wymaga podania danych modułowych (jeśli moduł tego wymaga).
   */
  public static async acceptInvoice(
    invoiceId: string,
    data: AcceptInvoiceRequest,
  ) {
    return await AxiosWrapper.post<string | null>(
      ApiUrl.AccountingAcceptInvoice(invoiceId),
      data,
    );
  }

  /**
   * Odrzuca fakturę i usuwa powiązany wpis z modułu (jeśli istnieje).
   * Zmienia status faktury na "Rejected".
   */
  public static async rejectInvoice(invoiceId: string) {
    return await AxiosWrapper.post(
      ApiUrl.AccountingRejectInvoice(invoiceId),
      {},
    );
  }

  /**
   * Wstrzymuje fakturę i przypisuje ją do innego pracownika (nie zmienia statusu)
   */
  public static async holdInvoice(invoiceId: string, data: HoldInvoiceData) {
    return await AxiosWrapper.post(
      ApiUrl.AccountingHoldInvoice(invoiceId),
      data,
    );
  }

  /**
   * Aktualizuje fermę w powiązanej encji modułu
   */
  public static async updateModuleEntityFarm(
    entityInvoiceId: string,
    moduleType: string,
    newFarmId: string,
  ) {
    return await AxiosWrapper.patch(
      `${ApiUrl.AccountingInvoices}/${entityInvoiceId}/module-entity/farm`,
      {
        moduleType,
        farmId: newFarmId,
      },
    );
  }

  /**
   * Usuwa encję z modułu powiązaną z fakturą
   */
  public static async deleteModuleEntity(
    entityInvoiceId: string,
    moduleType: string,
  ) {
    return await AxiosWrapper.delete(
      `${ApiUrl.AccountingInvoices}/${entityInvoiceId}/module-entity`,
      {
        moduleType,
      },
    );
  }

  /**
   * Pobiera pliki faktur jako ZIP
   */
  public static async downloadInvoicesZip(invoiceIds: string[]): Promise<Blob> {
    const response = await axios.post(
      ApiUrl.AccountingDownloadZip,
      invoiceIds,
      {
        responseType: "blob",
      },
    );
    return response.data;
  }

  // ==================== Attachments ====================

  /**
   * Przesyła załącznik do faktury
   */
  public static async uploadAttachment(invoiceId: string, file: File) {
    const formData = new FormData();
    formData.append("file", file);

    return await AxiosWrapper.post<InvoiceAttachment>(
      ApiUrl.AccountingInvoiceAttachments(invoiceId),
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      },
    );
  }

  /**
   * Pobiera listę załączników faktury
   */
  public static async getAttachments(invoiceId: string) {
    return await AxiosWrapper.get<InvoiceAttachment[]>(
      ApiUrl.AccountingInvoiceAttachments(invoiceId),
    );
  }

  /**
   * Pobiera załącznik faktury jako blob
   */
  public static async downloadAttachment(
    invoiceId: string,
    attachmentId: string,
  ): Promise<Blob> {
    const response = await axios.get(
      ApiUrl.AccountingInvoiceAttachment(invoiceId, attachmentId),
      {
        responseType: "blob",
      },
    );
    return response.data;
  }

  /**
   * Usuwa załącznik faktury
   */
  public static async deleteAttachment(
    invoiceId: string,
    attachmentId: string,
  ) {
    return await AxiosWrapper.delete(
      ApiUrl.AccountingInvoiceAttachment(invoiceId, attachmentId),
    );
  }

  // ==================== Audit Logs ====================

  /**
   * Pobiera historię audytu faktury
   */
  public static async getAuditLogs(invoiceId: string) {
    return await AxiosWrapper.get<InvoiceAuditLog[]>(
      ApiUrl.AccountingInvoiceAuditLogs(invoiceId),
    );
  }
}

export interface InvoiceAttachment {
  id: string;
  fileName: string;
  fileSize: number;
  contentType: string;
  uploadedAt: string;
  uploadedByName?: string;
}

export interface InvoiceAuditLog {
  id: string;
  action: string;
  actionDescription: string;
  previousStatus?: string;
  newStatus?: string;
  userName: string;
  comment?: string;
  createdAt: string;
}

export interface HoldInvoiceData {
  newAssignedUserId: string;
  expectedCurrentAssignedUserId: string | null;
}

// Types for module entity creation
export interface CreateFeedInvoiceFromKSeFData {
  invoiceId: string;
  farmId: string;
  cycleId: string;
  henhouseId?: string;
  invoiceNumber: string;
  bankAccountNumber: string;
  vendorName: string;
  itemName: string;
  quantity: number;
  unitPrice: number;
  invoiceDate: string;
  dueDate: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  comment?: string;
}

export interface CreateGasDeliveryFromKSeFData {
  invoiceId: string;
  farmId: string;
  contractorId?: string;
  contractorNip?: string;
  contractorName?: string;
  invoiceNumber: string;
  invoiceDate: string;
  invoiceTotal: number;
  unitPrice: number;
  quantity: number;
  comment?: string;
}

export interface CreateExpenseProductionFromKSeFData {
  invoiceId: string;
  farmId: string;
  cycleId: string;
  expenseContractorId?: string;
  expenseTypeId?: string;
  contractorNip?: string;
  contractorName?: string;
  invoiceNumber: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  invoiceDate: string;
  comment?: string;
}

export interface CreateSaleInvoiceFromKSeFData {
  invoiceId: string;
  farmId: string;
  cycleId: string;
  slaughterhouseId?: string;
  slaughterhouseNip?: string;
  slaughterhouseName?: string;
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
}

export interface CreateModuleEntityRequest {
  moduleType: string;
  feedData?: CreateFeedInvoiceFromKSeFData;
  gasData?: CreateGasDeliveryFromKSeFData;
  expenseData?: CreateExpenseProductionFromKSeFData;
  saleData?: CreateSaleInvoiceFromKSeFData;
}

export interface AcceptInvoiceRequest {
  moduleType: string;
  feedData?: CreateFeedInvoiceFromKSeFData;
  gasData?: CreateGasDeliveryFromKSeFData;
  expenseData?: CreateExpenseProductionFromKSeFData;
  saleData?: CreateSaleInvoiceFromKSeFData;
}
