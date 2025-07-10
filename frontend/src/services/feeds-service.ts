import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { FeedsDictionary } from "../models/feeds/feeds-dictionary";
import type { FeedsNamesQueryResponse } from "../models/feeds/feeds-names";
import type {
  AddFeedPriceFormData,
  FeedPriceListModel,
  UpdateFeedPriceFormData,
} from "../models/feeds/prices/feed-price";
import type { FeedsPricesFilterPaginationModel } from "../models/feeds/prices/price-filters";
import AxiosWrapper from "../utils/axios/wrapper";

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
    filters: FeedsPricesFilterPaginationModel
  ) {
    return await AxiosWrapper.get<PaginateModel<FeedPriceListModel>>(
      ApiUrl.FeedsPrices,
      {
        ...filters,
      }
    );
  }

  public static async updateFeedPrice(
    id: string,
    data: UpdateFeedPriceFormData
  ) {
    return await AxiosWrapper.patch(ApiUrl.UpdateFeedPrice + "/" + id, data);
  }

  public static async deleteFeedPrice(id: string) {
    return await AxiosWrapper.delete(ApiUrl.DeleteFeedPrice + "/" + id);
  }
}
