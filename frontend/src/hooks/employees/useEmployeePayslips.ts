import { useCallback, useState, useEffect } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { EmployeePayslipsService } from "../../services/employee-payslips-service";
import type {
  EmployeePayslipAggregation,
  EmployeePayslipListModel,
} from "../../models/employees/employees-payslips";
import type { EmployeePayslipsFilterPaginationModel } from "../../models/employees/employees-payslips-filters";

export function useEmployeePayslips(
  filters: EmployeePayslipsFilterPaginationModel
) {
  const [payslips, setPayslips] = useState<EmployeePayslipListModel[]>([]);
  const [totalRows, setTotalRows] = useState(0);
  const [aggregation, setAggregation] = useState<
    EmployeePayslipAggregation | undefined
  >(undefined);
  const [loading, setLoading] = useState(false);

  const fetchPayslips = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse(
        () => EmployeePayslipsService.getPayslips(filters),
        (data) => {
          setPayslips(data.responseData?.list.items ?? []);
          setTotalRows(data.responseData?.list.totalRows ?? 0);
          setAggregation(data.responseData?.aggregation);
        },
        undefined,
        "Nie udało się pobrać listy wypłat"
      );
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    fetchPayslips();
  }, [fetchPayslips]);

  return {
    payslips,
    totalRows,
    aggregation,
    loading,
    fetchPayslips,
  };
}
