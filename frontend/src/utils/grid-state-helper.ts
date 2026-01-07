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
