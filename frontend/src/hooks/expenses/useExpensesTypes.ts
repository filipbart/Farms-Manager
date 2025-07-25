import { useCallback, useState } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { ExpensesService } from "../../services/expenses-service";
import type { ExpenseTypeRow } from "../../models/expenses/expenses-types";

export function useExpensesTypes() {
  const [expensesTypes, setExpensesTypes] = useState<ExpenseTypeRow[]>([]);
  const [loadingExpensesTypes, setLoadingExpensesTypes] = useState(false);

  const fetchExpensesTypes = useCallback(async () => {
    setLoadingExpensesTypes(true);
    try {
      await handleApiResponse(
        () => ExpensesService.getExpensesTypes(),
        (data) => setExpensesTypes(data.responseData?.types ?? []),
        undefined,
        "Nie udało się pobrać listy dodatkowych pól"
      );
    } finally {
      setLoadingExpensesTypes(false);
    }
  }, []);

  return { expensesTypes, loadingExpensesTypes, fetchExpensesTypes };
}
