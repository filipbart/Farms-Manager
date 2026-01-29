import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { UpdateCorrectionData } from "../models/feeds/corrections/correction";
import type { FeedsDeliveriesFilterPaginationModel } from "../models/feeds/deliveries/deliveries-filters";
import type { DraftFeedInvoice } from "../models/feeds/deliveries/draft-feed-invoice";
import type {
  FeedDeliveryListModel,
  FeedInvoiceData,
} from "../models/feeds/deliveries/feed-invoice";
import type { FeedsDictionary } from "../models/feeds/feeds-dictionary";
import type { FeedsNamesQueryResponse } from "../models/feeds/feeds-names";
import type { FeedPaymentListModel } from "../models/feeds/payments/payment";
import type { FeedsPaymentsFilterPaginationModel } from "../models/feeds/payments/payments-filters";
import type {
  AddFeedPriceFormData,
  FeedPriceListModel,
  UpdateFeedPriceFormData,
} from "../models/feeds/prices/feed-price";
import type { FeedsPricesFilterPaginationModel } from "../models/feeds/prices/price-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export interface UploadDeliveriesFilesResponse {
  files: DraftFeedInvoice[];
}

export interface SaveFeedInvoiceData {
  filePath: string;
  draftId: string;
  data: FeedInvoiceData;
}

export interface AddFeedCorrectionData {
  invoiceNumber: string;
  farmId: string;
  cycleId: string;
  subTotal: number;
  vatAmount: number;
  invoiceTotal: number;
  invoiceDate: string;
  file: File | undefined;
  feedInvoiceIds: string[];
}

export class FeedsService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<FeedsDictionary>(ApiUrl.FeedsDict);
  }

  public static async getFeedsNames() {
    return await AxiosWrapper.get<FeedsNamesQueryResponse>(ApiUrl.FeedsNames);
  }

  public static async addFeedName(names: string[]) {
    return await AxiosWrapper.post(ApiUrl.AddFeedName, {
      names,
    });
  }

  public static async deleteFeedName(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteFeedName + "/" + id);
  }

  public static async addFeedPrice(data: AddFeedPriceFormData) {
    return await AxiosWrapper.post(ApiUrl.AddFeedPrice, data);
  }

  public static async getFeedsPrices(
    filters: FeedsPricesFilterPaginationModel,
  ) {
    return await AxiosWrapper.get<PaginateModel<FeedPriceListModel>>(
      ApiUrl.FeedsPrices,
      {
        ...filters,
      },
    );
  }

  public static async updateFeedPrice(
    id: string,
    data: UpdateFeedPriceFormData,
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateFeedPrice + "/" + id, data);
  }

  public static async deleteFeedPrice(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteFeedPrice + "/" + id);
  }

  public static async uploadInvoices(files: File[], signal: AbortSignal) {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });

    return await AxiosWrapper.post<UploadDeliveriesFilesResponse>(
      ApiUrl.UploadDeliveries,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        signal: signal,
      },
    );
  }

  public static async saveFeedInvoice(invoiceData: SaveFeedInvoiceData) {
    return await AxiosWrapper.post(ApiUrl.SaveInvoiceData, invoiceData);
  }

  public static async getFeedsPayments(
    filters: FeedsPaymentsFilterPaginationModel,
  ) {
    return await AxiosWrapper.get<PaginateModel<FeedPaymentListModel>>(
      ApiUrl.FeedsPayments,
      { ...filters },
    );
  }

  public static async deleteFeedPayment(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteFeedPayment + "/" + id);
  }

  public static async markPaymentAsCompleted(
    id: string,
    data: { comment?: string; paymentDate: string },
  ) {
    return await AxiosWrapper.patch(
      ApiUrl.MarkPaymentAsCompleted + "/" + id,
      data,
    );
  }

  public static async getFeedsDeliveries(
    filters: FeedsDeliveriesFilterPaginationModel,
  ) {
    return await AxiosWrapper.get<PaginateModel<FeedDeliveryListModel>>(
      ApiUrl.FeedsDeliveries,
      {
        ...filters,
      },
    );
  }

  public static async deleteFeedDelivery(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteFeedDelivery + "/" + id);
  }

  public static async updateFeedDelivery(
    id: string,
    data: FeedDeliveryListModel,
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateFeedDelivery + "/" + id, data);
  }

  public static async addFeedCorrection(dto: AddFeedCorrectionData) {
    const formData = new FormData();

    formData.append("farmId", dto.farmId);
    formData.append("cycleId", dto.cycleId);
    formData.append("invoiceNumber", dto.invoiceNumber);
    formData.append("subTotal", dto.subTotal.toString());
    formData.append("vatAmount", dto.vatAmount.toString());
    formData.append("invoiceTotal", dto.invoiceTotal.toString());
    formData.append("invoiceDate", dto.invoiceDate);

    if (dto.file) {
      formData.append("file", dto.file);
    }

    dto.feedInvoiceIds.forEach((id: string) => {
      formData.append("feedInvoiceIds", id);
    });
    return await AxiosWrapper.post(ApiUrl.AddFeedCorrection, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  }

  public static async deleteFeedCorrection(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteFeedCorrection + "/" + id);
  }

  public static async updateFeedCorrection(dto: UpdateCorrectionData) {
    return await AxiosWrapper.patch(ApiUrl.UpdateFeedCorrection, dto);
  }
}
