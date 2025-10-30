import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { SaleListModel, SaleType } from "../models/sales/sales";
import type { SalesDictionary } from "../models/sales/sales-dictionary";
import type { SalesFilterPaginationModel } from "../models/sales/sales-filters";
import type {
  SalesInvoiceListModel,
  SaveSalesInvoiceData,
  UpdateSalesInvoiceData,
  UploadSalesInvoicesFilesResponse,
} from "../models/sales/sales-invoices";
import type { SalesInvoicesFilterPaginationModel } from "../models/sales/sales-invoices-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export interface AddSaleData {
  saleType: SaleType;
  farmId: string;
  cycleId: string;
  saleDate: string;
  slaughterhouseId: string;
  files: File[];
  entries: {
    henhouseId: string;
    quantity: number;
    weight: number;
    confiscatedCount: number;
    confiscatedWeight: number;
    deadCount: number;
    deadWeight: number;
    farmerWeight: number;
    basePrice: number;
    priceWithExtras: number;
    comment?: string;
    otherExtras?: {
      name: string;
      value: number;
    }[];
  }[];
}

export interface AddNewSaleResponse {
  internalGroupId: string;
}

export interface BookInvoicesPaymentData {
  invoicesIds: string[];
  paymentDate: string;
}

export class SalesService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<SalesDictionary>(ApiUrl.SalesDict);
  }

  public static async getSales(filters: SalesFilterPaginationModel) {
    return await AxiosWrapper.get<PaginateModel<SaleListModel>>(ApiUrl.Sales, {
      ...filters,
    });
  }

  public static async addNewSale(data: AddSaleData) {
    const formData = new FormData();

    formData.append("saleType", data.saleType);
    formData.append("farmId", data.farmId);
    formData.append("cycleId", data.cycleId);
    formData.append("saleDate", data.saleDate);
    formData.append("slaughterhouseId", data.slaughterhouseId);

    formData.append("entries", JSON.stringify(data.entries));

    data.files.forEach((file) => {
      formData.append("files", file);
    });

    return await AxiosWrapper.post<AddNewSaleResponse>(
      ApiUrl.Sales + "/add",
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
  }

  public static async updateSale(saleId: string, payload: any, file?: File) {
    const formData = new FormData();
    
    // Append all fields to FormData
    Object.keys(payload).forEach((key) => {
      if (key === "otherExtras") {
        // OtherExtras must be JSON string
        formData.append(key, JSON.stringify(payload[key]));
      } else {
        // Convert numbers to strings with comma as decimal separator
        const value = typeof payload[key] === 'number' 
          ? payload[key].toString().replace('.', ',')
          : payload[key];
        formData.append(key, value);
      }
    });

    // Append file if provided
    if (file) {
      formData.append("file", file);
    }

    return await AxiosWrapper.post(
      ApiUrl.UpdateSale + "/" + saleId,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
  }

  public static async deleteSale(saleId: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteSale(saleId));
  }

  public static async sendToIrzPlus(payload: {
    internalGroupId?: string;
    saleId?: string;
  }) {
    return await AxiosWrapper.post(ApiUrl.SaleSendToIrz, payload);
  }

  public static async getSalesInvoices(
    filters: SalesInvoicesFilterPaginationModel
  ) {
    return await AxiosWrapper.get<PaginateModel<SalesInvoiceListModel>>(
      ApiUrl.SalesInvoices,
      { ...filters }
    );
  }

  public static async updateSaleInvoice(
    id: string,
    data: UpdateSalesInvoiceData
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateSaleInvoice(id), data);
  }

  public static async deleteSaleInvoice(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteSaveInvoice(id));
  }

  public static async uploadInvoices(files: File[], signal: AbortSignal) {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });

    return await AxiosWrapper.post<UploadSalesInvoicesFilesResponse>(
      ApiUrl.UploadSalesInvoices,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        signal: signal,
      }
    );
  }

  public static async saveSaleInvoice(invoiceData: SaveSalesInvoiceData) {
    return await AxiosWrapper.post(ApiUrl.SaveSalesInvoicesData, invoiceData);
  }

  public static async bookInvoicesPayment(data: BookInvoicesPaymentData) {
    return await AxiosWrapper.post(ApiUrl.BookSalesInvoicesPayment, data);
  }

  public static async markInvoiceAsCompleted(
    id: string,
    data: { paymentDate: string; comment?: string }
  ) {
    return await AxiosWrapper.post(
      ApiUrl.MarkInvoiceAsCompleted + "/" + id,
      data
    );
  }
}
