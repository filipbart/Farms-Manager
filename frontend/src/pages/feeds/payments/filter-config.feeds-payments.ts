import type { FilterConfig } from "../../../components/filters/filter-types";
import type { FeedsPaymentsFilterPaginationModel } from "../../../models/feeds/payments/payments-filters";

export const getFeedsPaymentsFiltersConfig = (): FilterConfig<
  keyof FeedsPaymentsFilterPaginationModel
>[] => [];
