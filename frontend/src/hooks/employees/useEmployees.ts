import { useCallback, useState } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type { EmployeeListModel } from "../../models/employees/employees";
import type { EmployeesFilterPaginationModel } from "../../models/employees/employees-filters";
import { EmployeesService } from "../../services/employees-service";

export function useEmployees(filters: EmployeesFilterPaginationModel) {
  const [employees, setEmployees] = useState<EmployeeListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchEmployees = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<EmployeeListModel>>(
        () => EmployeesService.getEmployees(filters),
        (data) => {
          setEmployees(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Nie udało się pobrać listy pracowników"
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  return {
    employees,
    totalRows,
    loading,
    fetchEmployees,
  };
}
