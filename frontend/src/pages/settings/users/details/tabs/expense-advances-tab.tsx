import {
  Box,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  FormControl,
  FormControlLabel,
  FormGroup,
  IconButton,
  MenuItem,
  Paper,
  Select,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  CircularProgress,
  InputLabel,
} from "@mui/material";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import AddIcon from "@mui/icons-material/Add";
import SaveIcon from "@mui/icons-material/Save";
import type { UserDetailsModel } from "../../../../../models/users/users";
import {
  ExpenseAdvancePermissionType,
  type AssignExpenseAdvancePermissionRequest,
  type UpdateExpenseAdvancePermissionRequest,
  type ExpenseAdvancePermission,
  type ExpenseAdvanceEntity,
  type AvailableColumn,
} from "../../../../../models/expenses/advances/expense-advance-permissions";
import { ExpenseAdvancePermissionsService } from "../../../../../services/expense-advance-permissions-service";
import { handleApiResponse } from "../../../../../utils/axios/handle-api-response";

interface ExpenseAdvancesTabProps {
  user: UserDetailsModel;
  refetch: () => void;
}

const ExpenseAdvancesTab: React.FC<ExpenseAdvancesTabProps> = ({
  user,
  refetch,
}) => {
  const [openDialog, setOpenDialog] = useState(false);
  const [editingPermission, setEditingPermission] =
    useState<ExpenseAdvancePermission | null>(null);
  const [selectedExpenseAdvanceId, setSelectedExpenseAdvanceId] = useState("");
  const [selectedPermissions, setSelectedPermissions] = useState<
    ExpenseAdvancePermissionType[]
  >([]);

  const [expenseAdvances, setExpenseAdvances] = useState<
    ExpenseAdvanceEntity[]
  >([]);
  const [permissions, setPermissions] = useState<ExpenseAdvancePermission[]>(
    []
  );
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);

  // Stan dla ustawień kolumn
  const [availableColumns, setAvailableColumns] = useState<AvailableColumn[]>(
    []
  );
  const [visibleColumns, setVisibleColumns] = useState<string[]>([]);
  const [savingColumns, setSavingColumns] = useState(false);

  // Pobieranie danych
  const fetchData = async () => {
    setLoading(true);
    try {
      const [advancesResponse, permissionsResponse, columnSettingsResponse] =
        await Promise.all([
          ExpenseAdvancePermissionsService.getAllExpenseAdvances(),
          ExpenseAdvancePermissionsService.getUserExpenseAdvancePermissions(
            user.id
          ),
          ExpenseAdvancePermissionsService.getUserColumnSettings(user.id),
        ]);

      if (advancesResponse.success) {
        const data = advancesResponse.responseData as any;
        // Backend zwraca obiekt {expenseAdvances: [...]} zamiast bezpośrednio tablicy
        const employees = data?.expenseAdvances || data;
        setExpenseAdvances(Array.isArray(employees) ? employees : []);
      } else {
        setExpenseAdvances([]);
      }

      if (permissionsResponse.success) {
        setPermissions(permissionsResponse.responseData?.permissions || []);
      }

      if (
        columnSettingsResponse.success &&
        columnSettingsResponse.responseData
      ) {
        setAvailableColumns(
          columnSettingsResponse.responseData.availableColumns || []
        );
        setVisibleColumns(
          columnSettingsResponse.responseData.visibleColumns || []
        );
      }
    } catch (err) {
      console.error("Błąd podczas pobierania danych", err);
      toast.error("Nie udało się pobrać danych");
    } finally {
      setLoading(false);
    }
  };

  // Zapisywanie ustawień kolumn
  const handleSaveColumnSettings = async () => {
    if (visibleColumns.length === 0) {
      toast.error("Musisz wybrać co najmniej jedną kolumnę");
      return;
    }

    setSavingColumns(true);
    try {
      await handleApiResponse(
        () =>
          ExpenseAdvancePermissionsService.updateUserColumnSettings({
            userId: user.id,
            visibleColumns,
          }),
        () => {
          toast.success("Ustawienia kolumn zostały zapisane");
        },
        undefined,
        "Nie udało się zapisać ustawień kolumn"
      );
    } catch {
      toast.error("Wystąpił błąd podczas zapisu ustawień kolumn");
    } finally {
      setSavingColumns(false);
    }
  };

  // Toggle widoczności kolumny
  const handleColumnToggle = (columnKey: string) => {
    setVisibleColumns((prev) =>
      prev.includes(columnKey)
        ? prev.filter((c) => c !== columnKey)
        : [...prev, columnKey]
    );
  };

  useEffect(() => {
    fetchData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user.id]);

  const handleOpenDialog = () => {
    setOpenDialog(true);
    setEditingPermission(null);
    setSelectedExpenseAdvanceId("");
    setSelectedPermissions([]);
  };

  const handleOpenEditDialog = (permission: ExpenseAdvancePermission) => {
    setOpenDialog(true);
    setEditingPermission(permission);
    setSelectedExpenseAdvanceId(permission.expenseAdvanceId);
    setSelectedPermissions([permission.permissionType]);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingPermission(null);
    setSelectedExpenseAdvanceId("");
    setSelectedPermissions([]);
  };

  const handlePermissionToggle = (permission: ExpenseAdvancePermissionType) => {
    setSelectedPermissions((prev) =>
      prev.includes(permission)
        ? prev.filter((p) => p !== permission)
        : [...prev, permission]
    );
  };

  const handleSave = async () => {
    if (!selectedExpenseAdvanceId || selectedPermissions.length === 0) {
      toast.error("Wybierz ewidencję i co najmniej jedno uprawnienie");
      return;
    }

    setSaving(true);
    try {
      if (editingPermission) {
        // Aktualizacja istniejących uprawnień
        const request: UpdateExpenseAdvancePermissionRequest = {
          permissionId: editingPermission.id,
          permissionTypes: selectedPermissions,
        };
        await handleApiResponse(
          () =>
            ExpenseAdvancePermissionsService.updateExpenseAdvancePermission(
              request
            ),
          () => {
            toast.success("Pomyślnie zaktualizowano uprawnienia");
            fetchData();
            refetch();
            handleCloseDialog();
          },
          undefined,
          "Nie udało się zaktualizować uprawnień"
        );
      } else {
        // Dodawanie nowych uprawnień
        const request: AssignExpenseAdvancePermissionRequest = {
          userId: user.id,
          expenseAdvanceId: selectedExpenseAdvanceId,
          permissionTypes: selectedPermissions,
        };
        await handleApiResponse(
          () =>
            ExpenseAdvancePermissionsService.assignExpenseAdvancePermission(
              request
            ),
          () => {
            toast.success("Pomyślnie przypisano uprawnienia");
            fetchData();
            refetch();
            handleCloseDialog();
          },
          undefined,
          "Nie udało się przypisać uprawnień"
        );
      }
    } catch {
      toast.error("Wystąpił błąd podczas zapisu");
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (permissionId: string) => {
    if (!window.confirm("Czy na pewno chcesz usunąć to uprawnienie?")) {
      return;
    }

    try {
      await handleApiResponse(
        () =>
          ExpenseAdvancePermissionsService.removeExpenseAdvancePermission({
            permissionId,
          }),
        () => {
          toast.success("Pomyślnie usunięto uprawnienie");
          fetchData();
          refetch();
        },
        undefined,
        "Nie udało się usunąć uprawnienia"
      );
    } catch {
      toast.error("Wystąpił błąd podczas usuwania");
    }
  };

  const getPermissionLabel = (type: ExpenseAdvancePermissionType): string => {
    switch (type) {
      case ExpenseAdvancePermissionType.View:
        return "Podgląd";
      case ExpenseAdvancePermissionType.Edit:
        return "Edycja";
      default:
        return type;
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={5}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box mt={2}>
      {/* Sekcja ustawień kolumn */}
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={2}
      >
        <Typography variant="h6">
          Widoczność kolumn w ewidencji zaliczek
        </Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={<SaveIcon />}
          onClick={handleSaveColumnSettings}
          disabled={savingColumns || visibleColumns.length === 0}
        >
          {savingColumns ? (
            <CircularProgress size={24} />
          ) : (
            "Zapisz ustawienia kolumn"
          )}
        </Button>
      </Box>

      <Paper sx={{ p: 2 }}>
        <Typography variant="body2" color="text.secondary" mb={2}>
          Wybierz kolumny, które użytkownik będzie widział w ewidencji zaliczek.
          Administrator oraz osoby ze wszystkimi uprawnieniami zawsze widzą
          wszystkie kolumny.
        </Typography>
        <FormGroup row>
          {availableColumns.map((column) => (
            <FormControlLabel
              key={column.key}
              control={
                <Checkbox
                  checked={visibleColumns.includes(column.key)}
                  onChange={() => handleColumnToggle(column.key)}
                />
              }
              label={column.description}
              sx={{ minWidth: 180 }}
            />
          ))}
        </FormGroup>
      </Paper>

      <Divider sx={{ my: 4 }} />

      {/* Sekcja uprawnień */}
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={2}
      >
        <Typography variant="h6">
          Uprawnienia do ewidencji zaliczek pracowników
        </Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={<AddIcon />}
          onClick={handleOpenDialog}
        >
          Dodaj uprawnienie
        </Button>
      </Box>

      {permissions.length === 0 ? (
        <Paper sx={{ p: 3, textAlign: "center" }}>
          <Typography variant="body2" color="text.secondary">
            Brak przypisanych uprawnień do ewidencji zaliczek
          </Typography>
        </Paper>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>
                  <strong>Pracownik (Ewidencja)</strong>
                </TableCell>
                <TableCell>
                  <strong>Typ uprawnienia</strong>
                </TableCell>
                <TableCell>
                  <strong>Data przypisania</strong>
                </TableCell>
                <TableCell align="right">
                  <strong>Akcje</strong>
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {permissions.map((permission) => (
                <TableRow key={permission.id}>
                  <TableCell>{permission.employeeName}</TableCell>
                  <TableCell>
                    {getPermissionLabel(permission.permissionType)}
                  </TableCell>
                  <TableCell>
                    {new Date(permission.createdAt).toLocaleDateString("pl-PL")}
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      size="small"
                      color="primary"
                      onClick={() => handleOpenEditDialog(permission)}
                      title="Edytuj"
                    >
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton
                      size="small"
                      color="error"
                      onClick={() => handleDelete(permission.id)}
                      title="Usuń"
                    >
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* Dialog dodawania/edycji uprawnień */}
      <Dialog
        open={openDialog}
        onClose={handleCloseDialog}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>
          {editingPermission
            ? "Edytuj uprawnienie"
            : "Dodaj uprawnienie do ewidencji"}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ display: "flex", flexDirection: "column", gap: 3, mt: 2 }}>
            <FormControl fullWidth>
              <InputLabel id="expense-advance-select-label">
                Wybierz ewidencję zaliczek (pracownika)
              </InputLabel>
              <Select
                labelId="expense-advance-select-label"
                value={selectedExpenseAdvanceId}
                onChange={(e) => setSelectedExpenseAdvanceId(e.target.value)}
                disabled={!!editingPermission}
                label="Wybierz ewidencję zaliczek (pracownika)"
              >
                <MenuItem value="" disabled>
                  Wybierz pracownika...
                </MenuItem>
                {Array.isArray(expenseAdvances) &&
                  expenseAdvances.map((ea) => (
                    <MenuItem key={ea.id} value={ea.id}>
                      {ea.employeeName}
                    </MenuItem>
                  ))}
              </Select>
            </FormControl>

            <FormControl component="fieldset">
              <Typography variant="body2" mb={1}>
                Wybierz uprawnienia:
              </Typography>
              <FormGroup>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={selectedPermissions.includes(
                        ExpenseAdvancePermissionType.View
                      )}
                      onChange={() =>
                        handlePermissionToggle(
                          ExpenseAdvancePermissionType.View
                        )
                      }
                    />
                  }
                  label="Podgląd (tylko odczyt)"
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={selectedPermissions.includes(
                        ExpenseAdvancePermissionType.Edit
                      )}
                      onChange={() =>
                        handlePermissionToggle(
                          ExpenseAdvancePermissionType.Edit
                        )
                      }
                    />
                  }
                  label="Edycja (możliwość zmian)"
                />
              </FormGroup>
            </FormControl>

            <Typography variant="caption" color="text.secondary">
              Możesz przypisać jedno lub oba uprawnienia. Edycja automatycznie
              daje również dostęp do podglądu.
            </Typography>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog} color="inherit">
            Anuluj
          </Button>
          <Button
            onClick={handleSave}
            variant="contained"
            color="primary"
            disabled={
              !selectedExpenseAdvanceId ||
              selectedPermissions.length === 0 ||
              saving
            }
          >
            {saving ? (
              <CircularProgress size={24} />
            ) : editingPermission ? (
              "Zapisz zmiany"
            ) : (
              "Dodaj uprawnienie"
            )}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default ExpenseAdvancesTab;
