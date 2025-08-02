import { useCallback, useState } from "react";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import type { PaginateModel } from "../../common/interfaces/paginate";
import { EmployeePayslipsService } from "../../services/employee-payslips-service";
import type { FarmPayslipRowModel } from "../../models/employees/payslips-farms";

export function useFarmsForPayslips() {
  const [payslipsFarms, setPayslipsFarms] = useState<FarmPayslipRowModel[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchPayslipsFarms = useCallback(async () => {
    setLoading(true);
    try {
      await handleApiResponse<PaginateModel<FarmPayslipRowModel>>(
        () => EmployeePayslipsService.getPayslipFarms(),
        (data) => {
          setPayslipsFarms(data.responseData?.items ?? []);
        },
        undefined,
        "Nie udało się pobrać listy słowników do rozliczenia"
      );
    } finally {
      setLoading(false);
    }
  }, []);

  return {
    payslipsFarms,
    loading,
    fetchPayslipsFarms,
  };
}
