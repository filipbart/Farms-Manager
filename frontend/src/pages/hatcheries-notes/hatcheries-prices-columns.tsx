import { Button } from "@mui/material";
import type { GridColDef } from "@mui/x-data-grid";
import dayjs from "dayjs";
import type { HatcheryPriceListModel } from "../../models/hatcheries/hatcheries-prices";

export const getHatcheriesPriceColumns = ({
  setSelectedHatcheryPrice,
  setIsEditModalOpen,
  deleteHatcheryPrice,
}: {
  setSelectedHatcheryPrice: (price: HatcheryPriceListModel) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
  deleteHatcheryPrice: (id: string) => void;
}): GridColDef<HatcheryPriceListModel>[] => {
  return [
    { field: "hatcheryName", headerName: "Wylęgarnia", flex: 1 },
    {
      field: "date",
      headerName: "Data",
      flex: 1,
      type: "string",
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD") : "";
      },
    },
    { field: "price", headerName: "Cena [zł]", flex: 1 },
    { field: "comment", headerName: "Komentarz", flex: 1 },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      flex: 1,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedHatcheryPrice(params.row);
            setIsEditModalOpen(true);
          }}
        >
          Edytuj
        </Button>,
        <Button
          key="delete"
          variant="outlined"
          size="small"
          color="error"
          onClick={() => {
            deleteHatcheryPrice(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
  ];
};
