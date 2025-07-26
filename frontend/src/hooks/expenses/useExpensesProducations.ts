import { useCallback, useState, useEffect } from "react";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type { ExpensesProductionsFilterPaginationModel } from "../../models/expenses/production/expenses-productions-filters";
import type { ExpenseProductionListModel } from "../../models/expenses/production/expenses-productions";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { ExpensesService } from "../../services/expenses-service";

export const useExpenseProductions = (
  filters: ExpensesProductionsFilterPaginationModel
) => {
  const [expenseProductions, setExpenseProductions] = useState<
    ExpenseProductionListModel[]
  >([]);
  const [totalRows, setTotalRows] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchExpenseProductions = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<ExpenseProductionListModel>>(
        () => ExpensesService.getExpensesProductions(filters),
        (data) => {
          setExpenseProductions(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Błąd podczas pobierania kosztów produkcji"
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchExpenseProductions();
  }, [fetchExpenseProductions]);

  return {
    expenseProductions,
    totalRows,
    loading,
    refetch: fetchExpenseProductions,
  };
};
