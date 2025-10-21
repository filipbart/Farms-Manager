import type { FilterConfig } from "../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { FeedsDeliveriesFilterPaginationModel } from "../../../models/feeds/deliveries/deliveries-filters";
import type { FeedsDictionary } from "../../../models/feeds/feeds-dictionary";
import type { FeedsNamesRow } from "../../../models/feeds/feeds-names";

export const getFeedsDeliveriesFiltersConfig = (
  dictionary: FeedsDictionary | undefined,
  uniqueCycles: CycleDictModel[],
  filters: FeedsDeliveriesFilterPaginationModel,
  feedsNames: FeedsNamesRow[],
  isAdmin: boolean = false
): FilterConfig<keyof FeedsDeliveriesFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<keyof FeedsDeliveriesFilterPaginationModel>[] = [
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
    key: "henhouseIds",
    label: "Kurnik",
    type: "multiSelect",
    options:
      dictionary?.farms
        .filter((farm) => filters.farmIds.includes(farm.id))
        .flatMap((farm) =>
          farm.henhouses.map((henhouse) => ({
            value: henhouse.id,
            label: henhouse.name,
          }))
        ) || [],
    disabled: !dictionary || filters.farmIds.length === 0,
  },
  {
    key: "feedNames",
    label: "Nazwa paszy",
    type: "multiSelect",
    options:
      feedsNames.map((feed) => ({
        value: feed.name,
        label: feed.name,
      })) || [],
    disabled: feedsNames.length === 0,
  },
  {
    key: "invoiceNumber",
    label: "Numer faktury",
    type: "text",
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
    key: "incorrectPrices",
    label: "Niepoprawna cena",
    type: "select",
    options: [
      { value: "true", label: "Tak" },
      { value: "false", label: "Nie" },
    ],
  },
  {
    key: "dateSince",
    label: "Data od",
    type: "date",
  },
  {
    key: "dateTo",
    label: "Data do",
    type: "date",
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
