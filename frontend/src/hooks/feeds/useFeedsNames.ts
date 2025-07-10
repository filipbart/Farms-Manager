import { useCallback, useState } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { FeedsService } from "../../services/feeds-service";
import type { FeedsNamesRow } from "../../models/feeds/feeds-names";

export function useFeedsNames() {
  const [feedsNames, setFeedsNames] = useState<FeedsNamesRow[]>([]);
  const [loadingFeedsNames, setLoadingFeedsNames] = useState(false);

  const fetchFeedsNames = useCallback(async () => {
    setLoadingFeedsNames(true);
    try {
      await handleApiResponse(
        () => FeedsService.getFeedsNames(),
        (data) => setFeedsNames(data.responseData?.fields ?? []),
        undefined,
        "Nie udało się pobrać listy dodatkowych pól"
      );
    } finally {
      setLoadingFeedsNames(false);
    }
  }, []);

  return { feedsNames, loadingFeedsNames, fetchFeedsNames };
}
