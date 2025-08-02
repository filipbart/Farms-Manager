import { useCallback, useState } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import type { PaginateModel } from "../../common/interfaces/paginate";
import type { EmployeePayslipListModel } from "../../models/employees/employees-payslips";
import { EmployeePayslipsService } from "../../services/employee-payslips-service";
import type { EmployeePayslipsFilterPaginationModel } from "../../models/employees/employees-payslips-filters";

export function useEmployeePayslips(
  filters: EmployeePayslipsFilterPaginationModel
) {
  const [payslips, setPayslips] = useState<EmployeePayslipListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchPayslips = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<EmployeePayslipListModel>>(
        () => EmployeePayslipsService.getPayslips(filters),
        (data) => {
          setPayslips(data.responseData?.items ?? []);
          setTotalRows(data.responseData?.totalRows ?? 0);
        },
        undefined,
        "Nie udało się pobrać listy wypłat"
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  return {
    payslips,
    totalRows,
    loading,
    fetchPayslips,
  };
}
