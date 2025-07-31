import { useCallback, useState } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import type { GasContractorRow } from "../../models/gas/gas-contractors";
import { GasService } from "../../services/gas-service";

export function useGasContractors() {
  const [gasContractors, setGasContractors] = useState<GasContractorRow[]>([]);
  const [loadingGasContractors, setLoadingGasContractors] = useState(false);

  const fetchGasContractors = useCallback(async () => {
    setLoadingGasContractors(true);
    try {
      await handleApiResponse(
        () => GasService.getGasContractors(),
        (data) => setGasContractors(data.responseData?.contractors ?? []),
        undefined,
        "Nie udało się pobrać listy kontrahentów"
      );
    } finally {
      setLoadingGasContractors(false);
    }
  }, []);

  return {
    gasContractors,
    loadingGasContractors,
    fetchGasContractors,
  };
}
