import ApiUrl from "../../common/ApiUrl";
import type {
  AddFallenStockPickups,
  GetFallenStockPickupsResponse,
} from "../../models/fallen-stocks/fallen-stock-pickups";
import type { FallenStockFilterModel } from "../../models/fallen-stocks/fallen-stocks-filters";
import AxiosWrapper from "../../utils/axios/wrapper";

export class FallenStockPickupService {
  public static async getFallenStockPickups(filters: FallenStockFilterModel) {
    return await AxiosWrapper.get<GetFallenStockPickupsResponse>(
      ApiUrl.FallenStockPickups,
      { ...filters }
    );
  }

  public static async addFallenStockPickups(data: AddFallenStockPickups) {
    return await AxiosWrapper.post(ApiUrl.AddFallenStockPickups, data);
  }

  public static async updateFallenStockPickup(
    fallenStockPickupId: string,
    quantity: number
  ) {
    return await AxiosWrapper.patch(
      ApiUrl.UpdateFallenStockPickup(fallenStockPickupId),
      { quantity }
    );
  }

  public static async deleteFallenStockPickup(fallenStockPickupId: string) {
    return await AxiosWrapper.delete(
      ApiUrl.DeleteFallenStockPickup(fallenStockPickupId)
    );
  }
}
