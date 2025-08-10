import {
  Toolbar,
  ToolbarButton,
  ColumnsPanelTrigger,
  FilterPanelTrigger,
  ExportCsv,
  ExportPrint,
  QuickFilter,
  QuickFilterControl,
  QuickFilterClear,
  QuickFilterTrigger,
} from "@mui/x-data-grid";
import Tooltip from "@mui/material/Tooltip";
import Menu from "@mui/material/Menu";
import Badge from "@mui/material/Badge";
import ViewColumnIcon from "@mui/icons-material/ViewColumn";
import FilterListIcon from "@mui/icons-material/FilterList";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import MenuItem from "@mui/material/MenuItem";
import TextField from "@mui/material/TextField";
import InputAdornment from "@mui/material/InputAdornment";
import CancelIcon from "@mui/icons-material/Cancel";
import SearchIcon from "@mui/icons-material/Search";
import { ExportExcel, useGridApiContext } from "@mui/x-data-grid-premium";
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  styled,
} from "@mui/material";
import { useState, useRef, useEffect } from "react";
import { MdDelete, MdSave } from "react-icons/md";
import { toast } from "react-toastify";

interface OwnerState {
  expanded: boolean;
}

const StyledQuickFilter = styled(QuickFilter)({
  display: "grid",
  alignItems: "center",
});

const StyledTextField = styled(TextField)<{
  ownerState: OwnerState;
}>(({ theme, ownerState }) => ({
  gridArea: "1 / 1",
  overflowX: "clip",
  width: ownerState.expanded ? 260 : "var(--trigger-width)",
  opacity: ownerState.expanded ? 1 : 0,
  transition: theme.transitions.create(["width", "opacity"]),
}));

const StyledToolbarButton = styled(ToolbarButton)<{ ownerState: OwnerState }>(
  ({ theme, ownerState }) => ({
    gridArea: "1 / 1",
    width: "min-content",
    height: "min-content",
    zIndex: 1,
    opacity: ownerState.expanded ? 0 : 1,
    pointerEvents: ownerState.expanded ? "none" : "auto",
    transition: theme.transitions.create(["opacity"]),
  })
);

interface SavedView {
  name: string;
  state: any;
}

const CustomToolbarAnalysis: React.FC = () => {
  const apiRef = useGridApiContext();
  const [exportMenuOpen, setExportMenuOpen] = useState(false);
  const exportMenuTriggerRef = useRef<HTMLButtonElement>(null);
  const [savedViews, setSavedViews] = useState<SavedView[]>([]);
  const [selectedView, setSelectedView] = useState<string>("");

  const [saveDialogOpen, setSaveDialogOpen] = useState(false);
  const [newViewName, setNewViewName] = useState("");

  useEffect(() => {
    const viewsFromStorage = localStorage.getItem("dataGridSavedViews");
    if (viewsFromStorage) {
      setSavedViews(JSON.parse(viewsFromStorage));
    }
  }, []);

  const handleSaveView = () => {
    setNewViewName("");
    setSaveDialogOpen(true);
  };

  const confirmSaveView = () => {
    if (!newViewName.trim()) {
      toast.error("Podaj nazwę widoku.");
      return;
    }

    const currentState = apiRef.current.exportState();
    const newView: SavedView = {
      name: newViewName.trim(),
      state: currentState,
    };

    const updatedViews = [
      ...savedViews.filter((v) => v.name !== newViewName.trim()),
      newView,
    ];

    setSavedViews(updatedViews);
    localStorage.setItem("dataGridSavedViews", JSON.stringify(updatedViews));
    toast.success(`Widok "${newViewName}" został zapisany.`);
    setSelectedView(newViewName);
    setSaveDialogOpen(false);
  };

  const handleLoadView = (viewName: string) => {
    setSelectedView(viewName);
    const viewToLoad = savedViews.find((v) => v.name === viewName);
    if (viewToLoad) {
      apiRef.current.restoreState(viewToLoad.state);
      toast.info(`Wczytano widok "${viewName}".`);
    }
  };

  const handleDeleteView = async (viewName: string) => {
    try {
      //TODO zapytać czy zapisywać lokalnie czy na uzytkownika
      // await api.delete(`/views/${viewName}`);

      const updatedViews = savedViews.filter((v) => v.name !== viewName);
      setSavedViews(updatedViews);
      localStorage.setItem("dataGridSavedViews", JSON.stringify(updatedViews));
      toast.success(`Widok "${viewName}" został usunięty.`);

      if (selectedView === viewName) {
        setSelectedView("");
      }
    } catch {
      toast.error("Nie udało się usunąć widoku.");
    }
  };

  return (
    <>
      <Toolbar>
        <ToolbarButton
          render={
            <Button
              startIcon={<MdSave />}
              onClick={handleSaveView}
              size="small"
              sx={{
                mr: 1,
                border: 1,
                borderColor: "primary.main",
              }}
            >
              Zapisz widok
            </Button>
          }
        />

        <TextField
          select
          label="Zapisane widoki"
          value={selectedView}
          onChange={(e) => handleLoadView(e.target.value)}
          variant="standard"
          size="small"
          sx={{ minWidth: 220, ml: 1, alignSelf: "center" }}
          slotProps={{
            select: {
              renderValue: (value) => {
                const view = savedViews.find((v) => v.name === value);
                return view ? view.name : <em>Wybierz widok...</em>;
              },
            },
          }}
        >
          <MenuItem value="" disabled>
            <em>Wybierz widok...</em>
          </MenuItem>
          {savedViews.map((view) => (
            <MenuItem
              key={view.name}
              value={view.name}
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
              }}
            >
              <span>{view.name}</span>
              <IconButton
                sx={{
                  ml: 1,
                  color: "error.main",
                  cursor: "pointer",
                  "&:hover": { color: "error.dark" },
                }}
                size="small"
                onClick={(e) => {
                  e.stopPropagation();
                  handleDeleteView(view.name);
                }}
              >
                <MdDelete fontSize="small" />
              </IconButton>
            </MenuItem>
          ))}
        </TextField>

        <Box sx={{ flexGrow: 1 }} />
        <Tooltip title="Kolumny">
          <ColumnsPanelTrigger render={<ToolbarButton />}>
            <ViewColumnIcon fontSize="small" />
          </ColumnsPanelTrigger>
        </Tooltip>
        <Tooltip title="Filtry">
          <FilterPanelTrigger
            render={(props, state) => (
              <ToolbarButton {...props} color="default">
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
        <Tooltip title="Eksportuj">
          <ToolbarButton
            ref={exportMenuTriggerRef}
            id="export-menu-trigger"
            aria-controls="export-menu"
            aria-haspopup="true"
            aria-expanded={exportMenuOpen ? "true" : undefined}
            onClick={() => setExportMenuOpen(true)}
          >
            <FileDownloadIcon fontSize="small" />
          </ToolbarButton>
        </Tooltip>
        <Menu
          id="export-menu"
          anchorEl={exportMenuTriggerRef.current}
          open={exportMenuOpen}
          onClose={() => setExportMenuOpen(false)}
          anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
          transformOrigin={{ vertical: "top", horizontal: "right" }}
          slotProps={{
            list: {
              "aria-labelledby": "export-menu-trigger",
            },
          }}
        >
          <ExportPrint
            render={<MenuItem />}
            onClick={() => setExportMenuOpen(false)}
          >
            Print
          </ExportPrint>
          <ExportCsv
            render={<MenuItem />}
            onClick={() => setExportMenuOpen(false)}
          >
            Download as CSV
          </ExportCsv>
          <ExportExcel render={<MenuItem />}>Download as Excel</ExportExcel>
        </Menu>
        <StyledQuickFilter>
          <QuickFilterTrigger
            render={(triggerProps, state) => (
              <Tooltip title="Search" enterDelay={0}>
                <StyledToolbarButton
                  {...triggerProps}
                  ownerState={{ expanded: state.expanded }}
                  color="default"
                  aria-disabled={state.expanded}
                >
                  <SearchIcon fontSize="small" />
                </StyledToolbarButton>
              </Tooltip>
            )}
          />
          <QuickFilterControl
            render={({ ref, ...controlProps }, state) => (
              <StyledTextField
                {...controlProps}
                ownerState={{ expanded: state.expanded }}
                inputRef={ref}
                aria-label="Search"
                placeholder="Search..."
                size="small"
                slotProps={{
                  input: {
                    startAdornment: (
                      <InputAdornment position="start">
                        <SearchIcon fontSize="small" />
                      </InputAdornment>
                    ),
                    endAdornment: state.value ? (
                      <InputAdornment position="end">
                        <QuickFilterClear
                          edge="end"
                          size="small"
                          aria-label="Clear search"
                          material={{ sx: { marginRight: -0.75 } }}
                        >
                          <CancelIcon fontSize="small" />
                        </QuickFilterClear>
                      </InputAdornment>
                    ) : null,
                    ...controlProps.slotProps?.input,
                  },
                  ...controlProps.slotProps,
                }}
              />
            )}
          />
        </StyledQuickFilter>
      </Toolbar>

      {/* Dialog zapisu widoku */}
      <Dialog open={saveDialogOpen} onClose={() => setSaveDialogOpen(false)}>
        <DialogTitle>Zapisz widok</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            label="Nazwa widoku"
            fullWidth
            variant="standard"
            value={newViewName}
            onChange={(e) => setNewViewName(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSaveDialogOpen(false)}>Anuluj</Button>
          <Button onClick={confirmSaveView} variant="contained">
            Zapisz
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default CustomToolbarAnalysis;
