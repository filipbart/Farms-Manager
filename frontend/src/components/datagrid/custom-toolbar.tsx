import {
  Toolbar,
  ToolbarButton,
  ColumnsPanelTrigger,
  FilterPanelTrigger,
  type GridToolbarProps,
} from "@mui/x-data-grid";
import Tooltip from "@mui/material/Tooltip";
import Badge from "@mui/material/Badge";
import Divider from "@mui/material/Divider";
import ToolbarSearch from "./toolbar-search";
import { MdFileDownload, MdFilterList, MdViewColumn } from "react-icons/md";
import Loading from "../loading/loading";

interface CustomToolbarProps extends GridToolbarProps {
  withSearch?: boolean;
  withExport?: boolean;
  onClickExport?: () => void;
  loadingExport?: boolean;
}

const CustomToolbar: React.FC<CustomToolbarProps> = ({
  withSearch,
  withExport,
  onClickExport,
  loadingExport = false,
}) => {
  return (
    <Toolbar>
      {withExport && (
        <Tooltip title="Eksport do CSV">
          {loadingExport ? (
            <Loading height="0" size={5} />
          ) : (
            <ToolbarButton style={{ color: "#374151" }} onClick={onClickExport}>
              <MdFileDownload />
            </ToolbarButton>
          )}
        </Tooltip>
      )}

      <Tooltip title="Kolumny">
        <ColumnsPanelTrigger
          render={<ToolbarButton style={{ color: "#374151" }} />}
        >
          <MdViewColumn />
        </ColumnsPanelTrigger>
      </Tooltip>

      <Tooltip title="Filtry">
        <FilterPanelTrigger
          render={(props, state) => (
            <ToolbarButton {...props} style={{ color: "#374151" }}>
              <Badge
                badgeContent={state.filterCount}
                color="primary"
                variant="dot"
              >
                <MdFilterList />
              </Badge>
            </ToolbarButton>
          )}
        />
      </Tooltip>

      {withSearch && (
        <>
          <Divider
            orientation="vertical"
            variant="middle"
            flexItem
            sx={{ mx: 0.5, color: "#374151" }}
          />
          <ToolbarSearch />
        </>
      )}
    </Toolbar>
  );
};
export default CustomToolbar;
