import type { FilterConfig } from "../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { FeedsDictionary } from "../../../models/feeds/feeds-dictionary";
import type { FeedsPaymentsFilterPaginationModel } from "../../../models/feeds/payments/payments-filters";

export const getFeedsPaymentsFiltersConfig = (
  dictionary: FeedsDictionary | undefined,
  uniqueCycles: CycleDictModel[],
  _filters: FeedsPaymentsFilterPaginationModel,
  isAdmin: boolean = false
): FilterConfig<keyof FeedsPaymentsFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<keyof FeedsPaymentsFilterPaginationModel>[] = [
    {
      key: "farmIds",
      label: "Ferma",
      type: "multiSelect",
      options:
        dictionary?.farms.map((farm) => ({
          value: farm.id,
          label: farm.name,
        })) || [],
      disabled: !dictionary,
    },
    {
      key: "cycles",
      label: "Identyfikator (cykl)",
      type: "multiSelect",
      options: uniqueCycles.map((cycle) => ({
        value: `${cycle.identifier}-${cycle.year}`,
        label: `${cycle.identifier}/${cycle.year}`,
      })),
      disabled: !dictionary,
    },
    {
      key: "status",
      label: "Status",
      type: "select",
      options: [
        { value: "Niezrealizowany", label: "Niezrealizowany" },
        { value: "Zrealizowany", label: "Zrealizowany" },
      ],
    },
  ];

  if (isAdmin) {
    baseFilters.push({
      key: "showDeleted",
      label: "Pokaż usunięte",
      type: "checkbox",
    });
  }

  return baseFilters;
};
