export interface PaginateModel<T = Record<string, any>> {
  totalRows: number;
  items: T[];
}
