import { Button } from "@mui/material";
import dayjs from "dayjs";
import type { ProductionDataRemainingFeedListModel } from "../../../models/production-data/remaining-feed";
import type { GridColDef } from "@mui/x-data-grid";

interface GetRemainingFeedColumnsProps {
  setSelectedRemainingFeed: (row: ProductionDataRemainingFeedListModel) => void;
  deleteRemainingFeed: (id: string) => void;
  setIsEditModalOpen: (isOpen: boolean) => void;
}

export const getRemainingFeedColumns = ({
  setSelectedRemainingFeed,
  deleteRemainingFeed,
  setIsEditModalOpen,
}: GetRemainingFeedColumnsProps): GridColDef<ProductionDataRemainingFeedListModel>[] => {
  return [
    {
      field: "id",
      headerName: "Id",
      width: 70,
    },
    {
      field: "cycleText",
      headerName: "Cykl",
      flex: 1,
    },
    {
      field: "farmName",
      headerName: "Ferma",
      flex: 1,
    },
    {
      field: "henhouseName",
      headerName: "Kurnik",
      flex: 1,
    },
    {
      field: "feedName",
      headerName: "Typ (nazwa) paszy",
      flex: 1,
    },
    {
      field: "remainingTonnage",
      headerName: "Tonaż pozostały [t]",
      flex: 1,
    },
    {
      field: "remainingValue",
      headerName: "Wartość [zł]",
      flex: 1,
    },
    {
      field: "actions",
      type: "actions",
      headerName: "Akcje",
      width: 200,
      getActions: (params) => [
        <Button
          key="edit"
          variant="outlined"
          size="small"
          onClick={() => {
            setSelectedRemainingFeed(params.row);
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
            deleteRemainingFeed(params.row.id);
          }}
        >
          Usuń
        </Button>,
      ],
    },
    {
      field: "dateCreatedUtc",
      headerName: "Data utworzenia wpisu",
      flex: 1,
      valueGetter: (value: string) => {
        return value ? dayjs(value).format("YYYY-MM-DD HH:mm") : "";
      },
    },
  ];
};
