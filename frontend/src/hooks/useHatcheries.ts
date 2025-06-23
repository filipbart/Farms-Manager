import { useCallback, useState } from "react";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { HatcheryRowModel } from "../models/hatcheries/hatchery-row-model";
import { HatcheriesService } from "../services/hatcheries-service";

export function useHatcheries() {
  const [hatcheries, setHatcheries] = useState<HatcheryRowModel[]>([]);
  const [loadingHatcheries, setLoadingHatcheries] = useState(false);

  const fetchHatcheries = useCallback(async () => {
    setLoadingHatcheries(true);
    try {
      await handleApiResponse<PaginateModel<HatcheryRowModel>>(
        () => HatcheriesService.getAllHatcheries(),
        (data) => setHatcheries(data.responseData?.items ?? []),
        undefined,
        "Nie udało się pobrać listy wylęgarni"
      );
    } finally {
      setLoadingHatcheries(false);
    }
  }, []);

  return { hatcheries, loadingHatcheries, fetchHatcheries };
}
