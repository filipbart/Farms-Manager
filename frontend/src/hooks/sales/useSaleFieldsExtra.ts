import { useCallback, useState } from "react";
import {
  SalesSettingsService,
  type SaleFieldsExtraRow,
} from "../../services/sales-settings-service";
import { handleApiResponse } from "../../utils/axios/handle-api-response";

export function useSaleFieldsExtra() {
  const [saleFieldsExtra, setSaleFieldsExtra] = useState<SaleFieldsExtraRow[]>(
    []
  );
  const [loadingSaleFieldsExtra, setLoadingSaleFieldsExtra] = useState(false);

  const fetchSaleFieldsExtra = useCallback(async () => {
    setLoadingSaleFieldsExtra(true);
    try {
      await handleApiResponse(
        () => SalesSettingsService.getSaleFieldsExtra(),
        (data) => setSaleFieldsExtra(data.responseData?.fields ?? []),
        undefined,
        "Nie udało się pobrać listy dodatkowych pól"
      );
    } finally {
      setLoadingSaleFieldsExtra(false);
    }
  }, []);

  return { saleFieldsExtra, loadingSaleFieldsExtra, fetchSaleFieldsExtra };
}
