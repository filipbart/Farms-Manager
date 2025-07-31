import { useCallback, useState, useEffect } from "react";
import type { PaginateModel } from "../../common/interfaces/paginate";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import type { GasDeliveryListModel } from "../../models/gas/gas-deliveries";
import type { GasDeliveriesFilterPaginationModel } from "../../models/gas/gas-deliveries-filters";
import { GasService } from "../../services/gas-service";

export const useGasDeliveries = (
  filters: GasDeliveriesFilterPaginationModel
) => {
  const [gasDeliveries, setGasDeliveries] = useState<GasDeliveryListModel[]>(
    []
  );
  const [totalRows, setTotalRows] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchGasDeliveries = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<GasDeliveryListModel>>(
        () => GasService.getGasDeliveries(filters),
        (data) => {
          setGasDeliveries(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania dostaw gazu"
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchGasDeliveries();
  }, [fetchGasDeliveries]);

  return {
    gasDeliveries,
    totalRows,
    loading,
    refetch: fetchGasDeliveries,
  };
};
