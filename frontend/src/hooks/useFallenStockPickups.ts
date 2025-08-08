import { useCallback, useState } from "react";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import type { FallenStockFilterModel } from "../models/fallen-stocks/fallen-stocks-filters";
import { FallenStockPickupService } from "../services/production-data/fallen-stock-pickups-service";
import type { FallenStockPickupRow } from "../models/fallen-stocks/fallen-stock-pickups";

export const useFallenStockPickups = (filters: FallenStockFilterModel) => {
  const [pickups, setPickups] = useState<FallenStockPickupRow[]>([]);
  const [loadingPickups, setLoadingPickups] = useState(false);

  const fetchPickups = useCallback(async () => {
    if (!filters.farmId || !filters.cycle) {
      setPickups([]);
      return;
    }
    setLoadingPickups(true);
    try {
      await handleApiResponse(
        () => FallenStockPickupService.getFallenStockPickups(filters),
        (data) => setPickups(data.responseData?.items ?? []),
        undefined,

        "Nie udało się pobrać listy odbiorów"
      );
    } finally {
      setLoadingPickups(false);
    }
  }, [filters]);

  return { pickups, loadingPickups, fetchPickups };
};
