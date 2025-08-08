export interface ColumnHeaderModel {
  id: string;
  name: string;
}

export interface TableRowModel {
  id: string;
  rowTitle: string;
  henhouseValues: Record<string, number | null>;
  remaining: number | null;
}

export interface FallenStockTableViewModel {
  henhouseColumns: ColumnHeaderModel[];
  insertionRows: TableRowModel[];
  summaryRows: TableRowModel[];
  grandTotal: number;
}
