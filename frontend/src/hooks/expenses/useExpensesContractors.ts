import { useCallback, useState } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { ExpensesService } from "../../services/expenses-service";
import type { ExpenseContractorRow } from "../../models/expenses/expenses-contractors";

export function useExpensesContractor() {
  const [expensesContractors, setExpensesContractors] = useState<
    ExpenseContractorRow[]
  >([]);
  const [loadingExpensesContractors, setLoadingExpensesContractors] =
    useState(false);

  const fetchExpensesContractors = useCallback(async () => {
    setLoadingExpensesContractors(true);
    try {
      await handleApiResponse(
        () => ExpensesService.getExpensesContractors(),
        (data) => setExpensesContractors(data.responseData?.contractors ?? []),
        undefined,
        "Nie udało się pobrać listy kontrahentów"
      );
    } finally {
      setLoadingExpensesContractors(false);
    }
  }, []);

  return {
    expensesContractors,
    loadingExpensesContractors,
    fetchExpensesContractors,
  };
}
