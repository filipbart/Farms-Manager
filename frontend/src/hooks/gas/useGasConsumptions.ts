import { useCallback, useState } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type { GasConsumptionListModel } from "../../models/gas/gas-consumptions";
import { GasService } from "../../services/gas-service";
import type { GasConsumptionsFilterPaginationModel } from "../../models/gas/gas-consumptions-filters";

export function useGasConsumptions(
  filters: GasConsumptionsFilterPaginationModel
) {
  const [gasConsumptions, setGasConsumptions] = useState<
    GasConsumptionListModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchGasConsumptions = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<GasConsumptionListModel>>(
        () => GasService.getGasConsumptions(filters),
        (data) => {
          setGasConsumptions(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Nie udało się pobrać listy zużycia gazu"
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  return {
    gasConsumptions,
    totalRows,
    loading,
    refetch: fetchGasConsumptions,
  };
}
