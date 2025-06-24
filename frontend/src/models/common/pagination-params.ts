export interface PaginationParams {
  pageNumber?: number;
  pageSize?: number;
}

export interface OrderedPaginationParams<T extends string = string>
  extends PaginationParams {
  orderBy?: T;
  isDescending?: boolean;
}

export interface OrderedPaginationParamsRaw extends PaginationParams {
  isDescending?: boolean;
}
