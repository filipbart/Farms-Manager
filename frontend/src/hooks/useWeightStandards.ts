import { useCallback, useState } from "react";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import type { WeightStandardRowModel } from "../models/production-data/weighings";
import { ProductionDataWeighingsService } from "../services/production-data/production-data-weighings-service";

export function useWeightStandards() {
  const [standards, setStandards] = useState<WeightStandardRowModel[]>([]);
  const [loadingStandards, setLoadingStandards] = useState(false);

  const fetchStandards = useCallback(async () => {
    setLoadingStandards(true);
    try {
      await handleApiResponse(
        () => ProductionDataWeighingsService.getStandards(),
        (data) => setStandards(data.responseData?.items ?? []),
        undefined,
        "Nie udało się pobrać listy norm wagowych"
      );
    } finally {
      setLoadingStandards(false);
    }
  }, []);

  return { standards, loadingStandards, fetchStandards };
}
