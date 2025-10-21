import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { AuditFields } from "../common/interfaces/audit-fields";

/**
 * Generuje kolumny audytowe dla DataGrid (widoczne tylko dla admina)
 * @param isAdmin - czy użytkownik jest adminem
 * @returns tablica kolumn audytowych
 */
export const getAuditColumns = <T extends AuditFields>(
  isAdmin: boolean
): GridColDef<T>[] => {
  if (!isAdmin) {
    return [];
  }

  return [
    {
      field: "createdByName",
      headerName: "Utworzył",
      width: 200,
      sortable: false,
      aggregable: false,
      valueGetter: (value: string | undefined) => {
        if (!value) return "-";
        return value;
      },
    },
    {
      field: "dateModifiedUtc",
      headerName: "Zmodyfikował (data)",
      width: 250,
      sortable: false,
      aggregable: false,
      valueGetter: (value: string | null | undefined, row: T) => {
        if (!value || !row.modifiedByName) return "-";
        const formattedDate = dayjs(value).format("YYYY-MM-DD HH:mm");
        return `${row.modifiedByName} (${formattedDate})`;
      },
    },
    {
      field: "dateDeletedUtc",
      headerName: "Usunął (data)",
      width: 250,
      sortable: false,
      aggregable: false,
      valueGetter: (value: string | null | undefined, row: T) => {
        if (!value || !row.deletedByName) return "-";
        const formattedDate = dayjs(value).format("YYYY-MM-DD HH:mm");
        return `${row.deletedByName} (${formattedDate})`;
      },
    },
  ];
};
