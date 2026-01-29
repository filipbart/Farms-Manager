import type { GridSortModel } from "@mui/x-data-grid-premium";

export interface SortOptions<T> {
  orderBy?: T;
  isDescending?: boolean;
}

/**
 * Inicjalizuje filtry z localStorage, zachowując stan paginacji i sortowania.
 * @param initialFilters Początkowy stan filtrów
 * @param gridStateKey Klucz dla stanu siatki w localStorage
 * @param pageSizeKey Klucz dla rozmiaru strony w localStorage
 * @param orderTypeEnum Enum z typami sortowania
 * @param mapOrderTypeToField Funkcja mapująca typ sortowania na pole
 * @returns Zainicjalizowany stan filtrów
 */
export const initializeFiltersFromLocalStorage = <T extends string>(
  initialFilters: any,
  gridStateKey: string,
  pageSizeKey: string,
  orderTypeEnum: { [s: string]: T },
  mapOrderTypeToField: (orderType: T) => string
) => {
  const savedState = localStorage.getItem(gridStateKey);
  const savedPageSize = localStorage.getItem(pageSizeKey);

  let initialized = { ...initialFilters };

  if (savedPageSize) {
    initialized.pageSize = parseInt(savedPageSize, 10);
  }

  if (savedState) {
    try {
      const state = JSON.parse(savedState);
      if (state.sorting && state.sorting.length > 0) {
        const sortOptions = getSortOptionsFromGridModel(
          state.sorting,
          orderTypeEnum,
          mapOrderTypeToField
        );
        initialized = { ...initialized, ...sortOptions, page: 0 };
      }
    } catch (e) {
      console.error("Error parsing saved grid state:", e);
    }
  }

  return initialized;
};

/**
 * Konwertuje model sortowania DataGrid na opcje sortowania specyficzne dla aplikacji.
 * @param model Model sortowania z DataGrid.
 * @param orderTypeEnum Enum z dostępnymi typami sortowania.
 * @param mapOrderTypeToField Funkcja mapująca typ sortowania na nazwę pola w siatce.
 * @returns Obiekt z opcjami sortowania (orderBy, isDescending).
 */
export const getSortOptionsFromGridModel = <T extends string>(
  model: GridSortModel,
  orderTypeEnum: { [s: string]: T },
  mapOrderTypeToField: (orderType: T) => string
): SortOptions<T> => {
  if (model.length > 0) {
    const { field, sort } = model[0];
    const foundOrderBy = Object.values(orderTypeEnum).find(
      (orderType) => mapOrderTypeToField(orderType) === field
    );

    if (foundOrderBy) {
      return { orderBy: foundOrderBy, isDescending: sort === "desc" };
    }
  }
  return { orderBy: undefined, isDescending: undefined };
};
