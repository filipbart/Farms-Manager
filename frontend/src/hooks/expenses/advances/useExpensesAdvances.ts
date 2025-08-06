import type { GetExpensesAdvancesResponse } from "../../../models/expenses/advances/expenses-advances";
import { ExpensesAdvancesService } from "../../../services/expenses-advances-service";
import type { ExpensesAdvancesFilterPaginationModel } from "../../../models/expenses/advances/expenses-advances-filters";
import { useState, useCallback, useEffect } from "react";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";

export const useExpenseAdvances = (
  employeeId: string | undefined,
  filters: ExpensesAdvancesFilterPaginationModel
) => {
  const [response, setResponse] = useState<GetExpensesAdvancesResponse>();
  const [loading, setLoading] = useState(false);

  const fetchExpenseAdvances = useCallback(async () => {
    if (!employeeId) {
      setResponse(undefined);
      return;
    }
    setLoading(true);
    try {
      await handleApiResponse(
        () => ExpensesAdvancesService.getExpensesAdvances(employeeId, filters),
        (data) => {
          if (data && data.responseData) setResponse(data.responseData);
        },
        undefined,
        "Błąd podczas pobierania zaliczek"
      );
    } finally {
      setLoading(false);
    }
  }, [employeeId, filters]);

  useEffect(() => {
    fetchExpenseAdvances();
  }, [fetchExpenseAdvances]);

  return {
    response,
    loading,
    fetchExpenseAdvances,
  };
};
