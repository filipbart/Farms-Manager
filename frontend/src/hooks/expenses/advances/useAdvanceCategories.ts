import { useCallback, useState } from "react";
import type { AdvanceCategoryModel } from "../../../models/expenses/advances/categories";
import { ExpensesAdvancesService } from "../../../services/expenses-advances-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";

export function useAdvanceCategories() {
  const [categories, setCategories] = useState<AdvanceCategoryModel[]>([]);
  const [loadingCategories, setLoadingCategories] = useState(false);

  const fetchCategories = useCallback(async () => {
    setLoadingCategories(true);
    try {
      await handleApiResponse(
        () => ExpensesAdvancesService.getAdvanceCategories(),
        (data) => setCategories(data.responseData ?? []),
        undefined,
        "Nie udało się pobrać listy kategorii"
      );
    } finally {
      setLoadingCategories(false);
    }
  }, []);

  return { categories, loadingCategories, fetchCategories };
}
