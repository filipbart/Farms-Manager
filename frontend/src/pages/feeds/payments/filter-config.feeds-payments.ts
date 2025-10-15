import type { FilterConfig } from "../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import type { FeedsDictionary } from "../../../models/feeds/feeds-dictionary";
import type { FeedsPaymentsFilterPaginationModel } from "../../../models/feeds/payments/payments-filters";
import {
  FeedPaymentStatus,
  FeedPaymentStatusLabels,
} from "../../../models/feeds/payments/payment";

export const getFeedsPaymentsFiltersConfig = (
  dictionary: FeedsDictionary | undefined,
  uniqueCycles: CycleDictModel[]
): FilterConfig<keyof FeedsPaymentsFilterPaginationModel>[] => [
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
    options: Object.values(FeedPaymentStatus).map((status) => ({
      value: status,
      label: FeedPaymentStatusLabels[status],
    })),
  },
];
