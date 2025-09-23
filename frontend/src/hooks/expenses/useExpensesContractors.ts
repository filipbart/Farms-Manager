import { useCallback, useState } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { ExpensesService } from "../../services/expenses-service";
import type {
  ExpenseContractorRow,
  ExpensesContractorsFilterPaginationModel,
} from "../../models/expenses/expenses-contractors";

export function useExpensesContractor(
  filters: ExpensesContractorsFilterPaginationModel
) {
  const [expensesContractors, setExpensesContractors] = useState<
    ExpenseContractorRow[]
  >([]);
  const [loadingExpensesContractors, setLoadingExpensesContractors] =
    useState(false);

  const fetchExpensesContractors = useCallback(async () => {
    setLoadingExpensesContractors(true);
    try {
      await handleApiResponse(
        () => ExpensesService.getExpensesContractors(filters),
        (data) => setExpensesContractors(data.responseData?.contractors ?? []),
        undefined,
        "Nie udało się pobrać listy kontrahentów"
      );
    } finally {
      setLoadingExpensesContractors(false);
    }
  }, [filters]);

  return {
    expensesContractors,
    loadingExpensesContractors,
    fetchExpensesContractors,
  };
}
