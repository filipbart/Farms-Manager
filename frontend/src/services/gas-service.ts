import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { GasContractorsQueryResponse } from "../models/gas/gas-contractors";
import type {
  AddGasDeliveryData,
  DraftGasInvoice,
  GasDeliveryListModel,
  SaveGasInvoiceData,
  UpdateGasDeliveryData,
} from "../models/gas/gas-deliveries";
import type { GasDeliveriesDictionary } from "../models/gas/gas-deliveries-dictionary";
import type { GasDeliveriesFilterPaginationModel } from "../models/gas/gas-deliveries-filters";
import AxiosWrapper from "../utils/axios/wrapper";

export interface UploadGasInvoicesResponse {
  files: DraftGasInvoice[];
}

export class GasService {
  public static async getDictionaries() {
    return await AxiosWrapper.get<GasDeliveriesDictionary>(
      ApiUrl.GasDeliveriesDictionary
    );
  }

  public static async getGasDeliveries(
    filters: GasDeliveriesFilterPaginationModel
  ) {
    return await AxiosWrapper.get<PaginateModel<GasDeliveryListModel>>(
      ApiUrl.GasDeliveries,
      { ...filters }
    );
  }

  public static async getGasContractors() {
    return await AxiosWrapper.get<GasContractorsQueryResponse>(
      ApiUrl.GasContractors
    );
  }

  public static async addGasDelivery(data: AddGasDeliveryData) {
    const formData = new FormData();

    formData.append("farmId", data.farmId);
    formData.append("contractorId", data.contractorId);
    formData.append("invoiceNumber", data.invoiceNumber);
    formData.append("invoiceDate", data.invoiceDate);
    formData.append("unitPrice", String(data.unitPrice));
    formData.append("quantity", String(data.quantity));
    formData.append("comment", data.comment);

    if (data.file) {
      formData.append("file", data.file);
    }

    return await AxiosWrapper.post(ApiUrl.AddGasDelivery, formData);
  }

  public static async updateGasDelivery(
    id: string,
    data: UpdateGasDeliveryData
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateGasDelivery(id), data);
  }

  public static async deleteGasDelivery(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteGasDelivery(id));
  }

  public static async uploadInvoices(files: File[]) {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });

    return await AxiosWrapper.post<UploadGasInvoicesResponse>(
      ApiUrl.UploadGasInvoices,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );
  }

  public static async saveGasInvoice(invoiceData: SaveGasInvoiceData) {
    return await AxiosWrapper.post(ApiUrl.SaveGasInvoiceData, invoiceData);
  }
}
