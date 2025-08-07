import { useCallback, useState } from "react";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { UtilizationPlantRowModel } from "../models/utilization-plants/utilization-plants";
import { UtilizationPlantsService } from "../services/utilization-plants-service";

export function useUtilizationPlants() {
  const [utilizationPlants, setUtilizationPlants] = useState<
    UtilizationPlantRowModel[]
  >([]);
  const [loadingUtilizationPlants, setLoadingUtilizationPlants] =
    useState(false);

  const fetchUtilizationPlants = useCallback(async () => {
    setLoadingUtilizationPlants(true);
    try {
      await handleApiResponse<PaginateModel<UtilizationPlantRowModel>>(
        () => UtilizationPlantsService.getAllUtilizationPlants(),
        (data) => setUtilizationPlants(data.responseData?.items ?? []),
        undefined,
        "Nie udało się pobrać listy zakładów utylizacyjnych"
      );
    } finally {
      setLoadingUtilizationPlants(false);
    }
  }, []);

  return {
    utilizationPlants,
    loadingUtilizationPlants,
    fetchUtilizationPlants,
  };
}
