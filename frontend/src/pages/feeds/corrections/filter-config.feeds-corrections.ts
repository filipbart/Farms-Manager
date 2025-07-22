import type { FilterConfig } from "../../../components/filters/filter-types";
import type { FeedsCorrectionsFilterPaginationModel } from "../../../models/feeds/corrections/corrections-filters";

export const getFeedsCorrectionsFiltersConfig = (): FilterConfig<
  keyof FeedsCorrectionsFilterPaginationModel
>[] => [];
