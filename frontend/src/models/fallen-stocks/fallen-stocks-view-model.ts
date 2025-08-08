export interface ColumnHeaderModel {
  id: string;
  name: string;
}

export interface TableRowModel {
  id: string;
  rowTitle: string;
  henhouseValues: Record<string, number | null>;
  remaining: number | null;
  isSentToIrz: boolean;
}

export interface FallenStockTableViewModel {
  henhouseColumns: ColumnHeaderModel[];
  insertionRows: TableRowModel[];
  summaryRows: TableRowModel[];
  grandTotal: number;
}
