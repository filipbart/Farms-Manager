import {
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  Button,
  MenuItem,
  Chip,
  Box,
  Typography,
  IconButton,
} from "@mui/material";
import { Add as AddIcon, Close as CloseIcon } from "@mui/icons-material";
import { useEffect, useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import { SettingsService } from "../../../services/settings-service";
import { UsersService } from "../../../services/users-service";
import { FarmsService } from "../../../services/farms-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import AppDialog from "../../common/app-dialog";
import LoadingButton from "../../common/loading-button";
import LoadingTextField from "../../common/loading-textfield";
import type {
  InvoiceAssignmentRule,
  CreateInvoiceAssignmentRuleDto,
  UpdateInvoiceAssignmentRuleDto,
} from "../../../models/settings/invoice-assignment-rule";
import type { UserListModel } from "../../../models/users/users";
import type FarmRowModel from "../../../models/farms/farm-row-model";

interface InvoiceAssignmentRuleModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  rule: InvoiceAssignmentRule | null;
}

interface FormData {
  name: string;
  description: string;
  assignedUserId: string;
  taxBusinessEntityId: string;
  farmIds: string[];
}

const InvoiceAssignmentRuleModal: React.FC<InvoiceAssignmentRuleModalProps> = ({
  open,
  onClose,
  onSave,
  rule,
}) => {
  const [loading, setLoading] = useState(false);
  const [loadingUsers, setLoadingUsers] = useState(false);
  const [loadingFarms, setLoadingFarms] = useState(false);
  const [users, setUsers] = useState<UserListModel[]>([]);
  const [farms, setFarms] = useState<FarmRowModel[]>([]);
  const [includeKeywords, setIncludeKeywords] = useState<string[]>([]);
  const [excludeKeywords, setExcludeKeywords] = useState<string[]>([]);
  const [newIncludeKeyword, setNewIncludeKeyword] = useState("");
  const [newExcludeKeyword, setNewExcludeKeyword] = useState("");

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    reset,
    setValue,
  } = useForm<FormData>({
    defaultValues: {
      name: "",
      description: "",
      assignedUserId: "",
      taxBusinessEntityId: "",
      farmIds: [],
    },
  });

  useEffect(() => {
    const fetchData = async () => {
      setLoadingUsers(true);
      setLoadingFarms(true);

      try {
        const [usersRes, farmsRes] = await Promise.all([
          UsersService.getUsers({ page: 0, pageSize: 100 }),
          FarmsService.getFarmsAsync(),
        ]);

        if (usersRes.success && usersRes.responseData) {
          setUsers(usersRes.responseData.items || []);
        }
        if (farmsRes.success && farmsRes.responseData) {
          setFarms(farmsRes.responseData.items || []);
        }
      } catch {
        // Ignore errors
      } finally {
        setLoadingUsers(false);
        setLoadingFarms(false);
      }
    };

    if (open) {
      fetchData();
    }
  }, [open]);

  useEffect(() => {
    if (rule) {
      setValue("name", rule.name);
      setValue("description", rule.description || "");
      setValue("assignedUserId", rule.assignedUserId);
      setValue("taxBusinessEntityId", rule.taxBusinessEntityId || "");
      setValue("farmIds", rule.farmIds || []);
      setIncludeKeywords(rule.includeKeywords || []);
      setExcludeKeywords(rule.excludeKeywords || []);
    } else {
      reset();
      setIncludeKeywords([]);
      setExcludeKeywords([]);
    }
  }, [rule, setValue, reset]);

  const handleClose = () => {
    reset();
    setIncludeKeywords([]);
    setExcludeKeywords([]);
    setNewIncludeKeyword("");
    setNewExcludeKeyword("");
    onClose();
  };

  const handleAddIncludeKeyword = () => {
    if (
      newIncludeKeyword.trim() &&
      !includeKeywords.includes(newIncludeKeyword.trim())
    ) {
      setIncludeKeywords([...includeKeywords, newIncludeKeyword.trim()]);
      setNewIncludeKeyword("");
    }
  };

  const handleAddExcludeKeyword = () => {
    if (
      newExcludeKeyword.trim() &&
      !excludeKeywords.includes(newExcludeKeyword.trim())
    ) {
      setExcludeKeywords([...excludeKeywords, newExcludeKeyword.trim()]);
      setNewExcludeKeyword("");
    }
  };

  const handleRemoveIncludeKeyword = (keyword: string) => {
    setIncludeKeywords(includeKeywords.filter((k) => k !== keyword));
  };

  const handleRemoveExcludeKeyword = (keyword: string) => {
    setExcludeKeywords(excludeKeywords.filter((k) => k !== keyword));
  };

  const handleSave = async (data: FormData) => {
    if (
      includeKeywords.length === 0 &&
      !data.taxBusinessEntityId &&
      data.farmIds.length === 0
    ) {
      toast.error(
        "Dodaj przynajmniej jedno słowo kluczowe lub wybierz działalność/lokalizację",
      );
      return;
    }

    setLoading(true);

    console.log("Saving rule with includeKeywords:", includeKeywords);
    console.log("Saving rule with excludeKeywords:", excludeKeywords);

    if (rule) {
      const updatePayload: UpdateInvoiceAssignmentRuleDto = {
        name: data.name,
        description: data.description || undefined,
        assignedUserId: data.assignedUserId,
        includeKeywords,
        excludeKeywords,
        taxBusinessEntityId: data.taxBusinessEntityId || undefined,
        farmIds: data.farmIds.length > 0 ? data.farmIds : [],
      };

      await handleApiResponse(
        () =>
          SettingsService.updateInvoiceAssignmentRule(rule.id, updatePayload),
        () => {
          toast.success("Reguła została zaktualizowana");
          handleClose();
          onSave();
        },
        undefined,
        "Błąd podczas aktualizacji reguły",
      );
    } else {
      const createPayload: CreateInvoiceAssignmentRuleDto = {
        name: data.name,
        description: data.description || undefined,
        assignedUserId: data.assignedUserId,
        includeKeywords,
        excludeKeywords,
        taxBusinessEntityId: data.taxBusinessEntityId || undefined,
        farmIds: data.farmIds,
      };

      await handleApiResponse(
        () => SettingsService.createInvoiceAssignmentRule(createPayload),
        () => {
          toast.success("Reguła została utworzona");
          handleClose();
          onSave();
        },
        undefined,
        "Błąd podczas tworzenia reguły",
      );
    }

    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="md">
      <DialogTitle>
        {rule
          ? "Edytuj regułę przypisywania"
          : "Dodaj nową regułę przypisywania"}
      </DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2.5} sx={{ mt: 0.5 }}>
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Nazwa reguły"
                fullWidth
                error={!!errors.name}
                helperText={errors.name?.message}
                {...register("name", {
                  required: "Nazwa jest wymagana",
                })}
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <TextField
                label="Opis (opcjonalny)"
                fullWidth
                multiline
                rows={2}
                {...register("description")}
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <Controller
                name="assignedUserId"
                control={control}
                rules={{ required: "Wybierz pracownika" }}
                render={({ field }) => (
                  <LoadingTextField
                    {...field}
                    select
                    label="Przypisz do pracownika"
                    fullWidth
                    loading={loadingUsers}
                    error={!!errors.assignedUserId}
                    helperText={errors.assignedUserId?.message}
                  >
                    {users.map((user) => (
                      <MenuItem key={user.id} value={user.id}>
                        {user.name}
                      </MenuItem>
                    ))}
                  </LoadingTextField>
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, md: 6 }}>
              <Controller
                name="farmIds"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    select
                    label="Lokalizacje (opcjonalne)"
                    fullWidth
                    disabled={loadingFarms}
                    SelectProps={{
                      multiple: true,
                      renderValue: (selected) => {
                        const selectedIds = selected as string[];
                        if (selectedIds.length === 0) {
                          return <em>Wszystkie lokalizacje</em>;
                        }
                        return selectedIds
                          .map((id) => farms.find((f) => f.id === id)?.name)
                          .filter(Boolean)
                          .join(", ");
                      },
                    }}
                  >
                    {farms.map((farm) => (
                      <MenuItem key={farm.id} value={farm.id}>
                        {farm.name}
                      </MenuItem>
                    ))}
                  </TextField>
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, md: 6 }}>
              <Controller
                name="taxBusinessEntityId"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    select
                    label="Działalność (opcjonalna)"
                    fullWidth
                  >
                    <MenuItem value="">
                      <em>Wszystkie działalności</em>
                    </MenuItem>
                  </TextField>
                )}
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <Typography variant="subtitle2" gutterBottom color="success.main">
                Słowa kluczowe (muszą wystąpić na fakturze)
              </Typography>
              <Box sx={{ display: "flex", gap: 1, mb: 1 }}>
                <TextField
                  size="small"
                  placeholder="Dodaj słowo kluczowe..."
                  value={newIncludeKeyword}
                  onChange={(e) => setNewIncludeKeyword(e.target.value)}
                  onKeyPress={(e) => {
                    if (e.key === "Enter") {
                      e.preventDefault();
                      handleAddIncludeKeyword();
                    }
                  }}
                  sx={{ flex: 1 }}
                />
                <IconButton
                  color="success"
                  onClick={handleAddIncludeKeyword}
                  disabled={!newIncludeKeyword.trim()}
                >
                  <AddIcon />
                </IconButton>
              </Box>
              <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
                {includeKeywords.map((keyword) => (
                  <Chip
                    key={keyword}
                    label={keyword}
                    color="success"
                    variant="outlined"
                    onDelete={() => handleRemoveIncludeKeyword(keyword)}
                    deleteIcon={<CloseIcon />}
                  />
                ))}
              </Box>
            </Grid>

            <Grid size={{ xs: 12 }}>
              <Typography variant="subtitle2" gutterBottom color="error.main">
                Słowa wykluczające (jeśli wystąpią, reguła nie zadziała)
              </Typography>
              <Box sx={{ display: "flex", gap: 1, mb: 1 }}>
                <TextField
                  size="small"
                  placeholder="Dodaj słowo wykluczające..."
                  value={newExcludeKeyword}
                  onChange={(e) => setNewExcludeKeyword(e.target.value)}
                  onKeyPress={(e) => {
                    if (e.key === "Enter") {
                      e.preventDefault();
                      handleAddExcludeKeyword();
                    }
                  }}
                  sx={{ flex: 1 }}
                />
                <IconButton
                  color="error"
                  onClick={handleAddExcludeKeyword}
                  disabled={!newExcludeKeyword.trim()}
                >
                  <AddIcon />
                </IconButton>
              </Box>
              <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.5 }}>
                {excludeKeywords.map((keyword) => (
                  <Chip
                    key={keyword}
                    label={keyword}
                    color="error"
                    variant="outlined"
                    onDelete={() => handleRemoveExcludeKeyword(keyword)}
                    deleteIcon={<CloseIcon />}
                  />
                ))}
              </Box>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} variant="outlined" color="inherit">
            Anuluj
          </Button>
          <LoadingButton
            type="submit"
            variant="contained"
            color="primary"
            startIcon={<MdSave />}
            loading={loading}
          >
            {rule ? "Zapisz zmiany" : "Utwórz regułę"}
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default InvoiceAssignmentRuleModal;
