import { useState, useCallback } from "react";
import type { FallenStockFilterModel } from "../models/fallen-stocks/fallen-stocks-filters";
import { FallenStockService } from "../services/production-data/fallen-stocks-service";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import type { FallenStockTableViewModel } from "../models/fallen-stocks/fallen-stocks-view-model";

export const useFallenStocks = (filters: FallenStockFilterModel) => {
  const [viewModel, setViewModel] = useState<FallenStockTableViewModel | null>(
    null
  );
  const [loading, setLoading] = useState(false);

  const fetchFallenStocks = useCallback(async () => {
    if (!filters.farmId || !filters.cycleId) {
      setViewModel(null);
      return;
    }

    setLoading(true);
    await handleApiResponse(
      () => FallenStockService.getFallenStocks(filters),
      (data) => {
        if (data && data.responseData) {
          setViewModel(data.responseData);
        }
      },
      undefined,
      "Błąd podczas pobierania danych do tabeli."
    );
    setLoading(false);
  }, [filters]);

  return {
    viewModel,
    loading,
    fetchFallenStocks,
  };
};
