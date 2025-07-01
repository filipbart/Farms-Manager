import { useCallback, useState } from "react";
import { handleApiResponse } from "../utils/axios/handle-api-response";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { SlaughterhouseRowModel } from "../models/slaughterhouses/slaughterhouse-row-model";
import { SlaughterhousesService } from "../services/slaughterhouses-service";

export function useSlaughterhouses() {
  const [slaughterhouses, setSlaughterhouses] = useState<
    SlaughterhouseRowModel[]
  >([]);
  const [loadingSlaughterhouses, setLoadingSlaughterhouses] = useState(false);

  const fetchSlaughterhouses = useCallback(async () => {
    setLoadingSlaughterhouses(true);
    try {
      await handleApiResponse<PaginateModel<SlaughterhouseRowModel>>(
        () => SlaughterhousesService.getAllSlaughterhouses(),
        (data) => setSlaughterhouses(data.responseData?.items ?? []),
        undefined,
        "Nie udało się pobrać listy ubojni"
      );
    } finally {
      setLoadingSlaughterhouses(false);
    }
  }, []);

  return { slaughterhouses, loadingSlaughterhouses, fetchSlaughterhouses };
}
