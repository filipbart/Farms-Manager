import { useCallback, useState, useEffect } from "react";
import type { PaginateModel } from "../../common/interfaces/paginate";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import type { SalesInvoiceListModel } from "../../models/sales/sales-invoices";
import { SalesService } from "../../services/sales-service";
import type { SalesInvoicesFilterPaginationModel } from "../../models/sales/sales-invoices-filters";

export const useSalesInvoices = (
  filters: SalesInvoicesFilterPaginationModel
) => {
  const [salesInvoices, setSalesInvoices] = useState<SalesInvoiceListModel[]>(
    []
  );
  const [totalRows, setTotalRows] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchSalesInvoices = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<SalesInvoiceListModel>>(
        () => SalesService.getSalesInvoices(filters),
        (data) => {
          setSalesInvoices(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania faktur sprzedaży"
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchSalesInvoices();
  }, [fetchSalesInvoices]);

  return {
    salesInvoices,
    totalRows,
    loading,
    fetchSalesInvoices,
  };
};
