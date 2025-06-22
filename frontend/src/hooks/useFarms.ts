import { useCallback, useState } from "react";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import type FarmRowModel from "../models/farms/farm-row-model";
import type { PaginateModel } from "../common/interfaces/paginate";
import { FarmsService } from "../services/farms-service";

export function useFarms() {
  const [farms, setFarms] = useState<FarmRowModel[]>([]);
  const [loadingFarms, setLoadingFarms] = useState(false);

  const fetchFarms = useCallback(async () => {
    setLoadingFarms(true);
    try {
      await handleApiResponse<PaginateModel<FarmRowModel>>(
        () => FarmsService.getFarmsAsync(),
        (data) => setFarms(data.responseData?.items ?? []),
        undefined,
        "Nie udało się pobrać listy farm"
      );
    } finally {
      setLoadingFarms(false);
    }
  }, []);

  return { farms, loadingFarms, fetchFarms };
}
