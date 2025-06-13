import {
  Toolbar,
  ToolbarButton,
  ColumnsPanelTrigger,
  FilterPanelTrigger,
  type GridToolbarProps,
} from "@mui/x-data-grid";
import Tooltip from "@mui/material/Tooltip";
import Badge from "@mui/material/Badge";
import ViewColumnIcon from "@mui/icons-material/ViewColumn";
import FilterListIcon from "@mui/icons-material/FilterList";
import Divider from "@mui/material/Divider";
import ToolbarSearch from "./toolbar-search";

interface CustomToolbarProps extends GridToolbarProps {
  withSearch?: boolean;
}

const CustomToolbar: React.FC<CustomToolbarProps> = ({ withSearch }) => {
  return (
    <Toolbar>
      <Tooltip title="Kolumny">
        <ColumnsPanelTrigger
          render={<ToolbarButton style={{ color: "#374151" }} />}
        >
          <ViewColumnIcon fontSize="small" />
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
                <FilterListIcon fontSize="small" />
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
