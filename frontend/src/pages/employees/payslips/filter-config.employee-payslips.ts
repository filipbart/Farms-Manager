import type { FilterConfig } from "../../../components/filters/filter-types";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { PayrollPeriod } from "../../../models/employees/employees-payslips";
import type {
  EmployeePayslipsDictionary,
  EmployeePayslipsFilterPaginationModel,
} from "../../../models/employees/employees-payslips-filters";

// Pomocnicza funkcja do mapowania enuma na polskie nazwy miesięcy
const getPayrollPeriodOptions = () => {
  const polishMonths = {
    [PayrollPeriod.January]: "Styczeń",
    [PayrollPeriod.February]: "Luty",
    [PayrollPeriod.March]: "Marzec",
    [PayrollPeriod.April]: "Kwiecień",
    [PayrollPeriod.May]: "Maj",
    [PayrollPeriod.June]: "Czerwiec",
    [PayrollPeriod.July]: "Lipiec",
    [PayrollPeriod.August]: "Sierpień",
    [PayrollPeriod.September]: "Wrzesień",
    [PayrollPeriod.October]: "Październik",
    [PayrollPeriod.November]: "Listopad",
    [PayrollPeriod.December]: "Grudzień",
  };

  return Object.values(PayrollPeriod).map((month) => ({
    value: month,
    label: polishMonths[month],
  }));
};

const getYearOptions = () => {
  const currentYear = new Date().getFullYear();
  const startYear = 2024;
  const years: { value: string; label: string }[] = [
    { value: "", label: "Wszystkie lata" },
  ];

  for (let year = currentYear; year >= startYear; year--) {
    years.push({ value: year.toString(), label: year.toString() });
  }

  return years;
};

export const getEmployeePayslipsFiltersConfig = (
  dictionary: EmployeePayslipsDictionary | undefined,
  uniqueCycles: CycleDictModel[],
  isAdmin: boolean = false
): FilterConfig<keyof EmployeePayslipsFilterPaginationModel>[] => {
  const baseFilters: FilterConfig<
    keyof EmployeePayslipsFilterPaginationModel
  >[] = [
    {
      key: "searchPhrase",
      label: "Szukaj pracownika...",
      type: "text",
    },
    {
      key: "farmIds",
      label: "Ferma",
      type: "multiSelect",
      options:
        dictionary?.farms.map((farm) => ({
          value: farm.id,
          label: farm.name,
        })) || [],
      disabled: !dictionary,
    },
    {
      key: "cycles",
      label: "Cykl",
      type: "multiSelect",
      options: uniqueCycles.map((cycle) => ({
        value: `${cycle.identifier}-${cycle.year}`,
        label: `${cycle.identifier}/${cycle.year}`,
      })),
      disabled: !dictionary,
    },
    {
      key: "payrollPeriod",
      label: "Okres rozliczeniowy",
      type: "select",
      options: getPayrollPeriodOptions(),
    },
    {
      key: "year",
      label: "Rok",
      type: "select",
      options: getYearOptions(),
    },
  ];

  if (isAdmin) {
    baseFilters.push({
      key: "showDeleted",
      label: "Pokaż usunięte",
      type: "checkbox",
    });
  }

  return baseFilters;
};
