import { useState } from "react";
import type CycleDto from "../models/farms/latest-cycle";
import { FarmsService } from "../services/farms-service";
import { handleApiResponse } from "../utils/axios/handle-api-response";

export const useLatestCycle = () => {
  const [loadingCycle, setLoadingCycle] = useState(false);

  const loadLatestCycle = async (farmId: string): Promise<CycleDto | null> => {
    setLoadingCycle(true);
    let result: CycleDto | null = null;

    await handleApiResponse(
      () => FarmsService.getLatestCycle(farmId),
      (data) => {
        if (!data?.responseData) {
          result = null;
          return;
        }

        const cycle = data.responseData as CycleDto;

        result = cycle;
      },
      undefined,
      "Nie udało się pobrać ostatniego cyklu"
    );

    setLoadingCycle(false);
    return result;
  };

  return { loadLatestCycle, loadingCycle };
};
