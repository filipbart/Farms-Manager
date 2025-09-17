import type { GridSortModel } from "@mui/x-data-grid-premium";

export interface SortOptions<T> {
  orderBy?: T;
  isDescending?: boolean;
}

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

/**
 * Inicjalizuje stan filtrów na podstawie danych zapisanych w localStorage.
 * @param initialFilters Początkowy obiekt filtrów.
 * @param gridStateKey Klucz localStorage dla stanu siatki.
 * @param pageSizeKey Klucz localStorage dla rozmiaru strony.
 * @param orderTypeEnum Enum z typami sortowania.
 * @param mapOrderTypeToField Funkcja mapująca typ sortowania na pole.
 * @returns Zainicjalizowany stan filtrów.
 */
export const initializeFiltersFromLocalStorage = <T extends string, K>(
  initialFilters: K,
  gridStateKey: string,
  pageSizeKey: string,
  orderTypeEnum: { [s: string]: T },
  mapOrderTypeToField: (orderType: T) => string
): K & SortOptions<T> & { pageSize: number } => {
  const savedPageSize = localStorage.getItem(pageSizeKey);
  const savedGridState = localStorage.getItem(gridStateKey);
  let sortOptions: SortOptions<T> = {};

  if (savedGridState) {
    try {
      const parsedGridState = JSON.parse(savedGridState);
      if (parsedGridState.sorting?.sortModel) {
        sortOptions = getSortOptionsFromGridModel(
          parsedGridState.sorting.sortModel,
          orderTypeEnum,
          mapOrderTypeToField
        );
      }
    } catch (e) {
      console.error(`Failed to parse ${gridStateKey} from localStorage`, e);
    }
  }

  return {
    ...initialFilters,
    pageSize: savedPageSize
      ? parseInt(savedPageSize, 10)
      : (initialFilters as any).pageSize ?? 10,
    ...sortOptions,
  };
};
