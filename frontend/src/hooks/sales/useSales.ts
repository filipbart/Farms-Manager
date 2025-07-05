import { useCallback, useState, useEffect } from "react";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type { SaleListModel } from "../../models/sales/sales";
import type { SalesFilterPaginationModel } from "../../models/sales/sales-filters";
import { SalesService } from "../../services/sales-service";
import { handleApiResponse } from "../../utils/axios/handle-api-response";

export const useSales = (filters: SalesFilterPaginationModel) => {
  const [sales, setSales] = useState<SaleListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchSales = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<SaleListModel>>(
        () => SalesService.getSales(filters),
        (data) => {
          setSales(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania sprzedaży"
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchSales();
  }, [fetchSales]);

  return { sales, totalRows, loading, refetch: fetchSales };
};
