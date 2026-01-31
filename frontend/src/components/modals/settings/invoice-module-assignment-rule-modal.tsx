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

import { FarmsService } from "../../../services/farms-service";

import { TaxBusinessEntitiesService } from "../../../services/tax-business-entities-service";

import { handleApiResponse } from "../../../utils/axios/handle-api-response";

import AppDialog from "../../common/app-dialog";

import LoadingButton from "../../common/loading-button";

import LoadingTextField from "../../common/loading-textfield";

import type {
  InvoiceModuleAssignmentRule,
  CreateInvoiceModuleAssignmentRuleDto,
  ModuleType,
  KSeFInvoiceDirection,
} from "../../../models/settings/invoice-module-assignment-rule";

import type FarmRowModel from "../../../models/farms/farm-row-model";

import type { TaxBusinessEntityRowModel } from "../../../models/data/tax-business-entity";

interface InvoiceModuleAssignmentRuleModalProps {
  open: boolean;

  onClose: () => void;

  onSave: () => void;

  rule: InvoiceModuleAssignmentRule | null;
}

interface FormData {
  name: string;

  description: string;

  targetModule: ModuleType;

  taxBusinessEntityId: string;

  farmId: string;

  invoiceDirection: string;
}

const moduleTypeOptions: { value: ModuleType; label: string }[] = [
  { value: "None", label: "Brak przypisanego" },

  { value: "Feeds", label: "Pasze" },

  { value: "ProductionExpenses", label: "Koszty produkcyjne" },

  { value: "Gas", label: "Gaz" },

  { value: "Sales", label: "Sprzedaże" },

  { value: "Farmstead", label: "Gospodarstwo rolne" },

  { value: "Investments", label: "Inwestycje" },

  { value: "RealEstate", label: "Nieruchomości" },

  { value: "Other", label: "Inne" },
];

const invoiceDirectionOptions: {
  value: KSeFInvoiceDirection;

  label: string;
}[] = [
  { value: "Purchase", label: "Zakup" },

  { value: "Sales", label: "Sprzedaż" },
];

const InvoiceModuleAssignmentRuleModal: React.FC<
  InvoiceModuleAssignmentRuleModalProps
> = ({ open, onClose, onSave, rule }) => {
  const [loading, setLoading] = useState(false);

  const [loadingFarms, setLoadingFarms] = useState(false);

  const [loadingTaxBusinessEntities, setLoadingTaxBusinessEntities] =
    useState(false);

  const [farms, setFarms] = useState<FarmRowModel[]>([]);

  const [taxBusinessEntities, setTaxBusinessEntities] = useState<
    TaxBusinessEntityRowModel[]
  >([]);

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

      targetModule: "Feeds",

      taxBusinessEntityId: "",

      farmId: "",

      invoiceDirection: "",
    },
  });

  useEffect(() => {
    const fetchData = async () => {
      setLoadingFarms(true);

      setLoadingTaxBusinessEntities(true);

      try {
        const [farmsRes, taxBusinessEntitiesRes] = await Promise.all([
          FarmsService.getFarmsAsync(),

          TaxBusinessEntitiesService.getAllAsync(),
        ]);

        if (farmsRes.success && farmsRes.responseData) {
          setFarms(farmsRes.responseData.items || []);
        }

        if (
          taxBusinessEntitiesRes.success &&
          taxBusinessEntitiesRes.responseData
        ) {
          setTaxBusinessEntities(
            taxBusinessEntitiesRes.responseData.items || [],
          );
        }
      } catch {
        // Ignore errors
      } finally {
        setLoadingFarms(false);

        setLoadingTaxBusinessEntities(false);
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

      setValue("targetModule", rule.targetModule);

      setValue("taxBusinessEntityId", rule.taxBusinessEntityId || "");

      setValue("farmId", rule.farmId || "");

      setValue("invoiceDirection", rule.invoiceDirection || "");

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
      !data.farmId &&
      !data.invoiceDirection
    ) {
      toast.error(
        "Dodaj przynajmniej jedno słowo kluczowe lub wybierz działalność/lokalizację/kierunek faktury",
      );

      return;
    }

    setLoading(true);

    const payload: CreateInvoiceModuleAssignmentRuleDto = {
      name: data.name,

      description: data.description || undefined,

      targetModule: data.targetModule,

      includeKeywords,

      excludeKeywords,

      taxBusinessEntityId: data.taxBusinessEntityId || undefined,

      farmId: data.farmId || undefined,

      invoiceDirection:
        (data.invoiceDirection as KSeFInvoiceDirection) || undefined,
    };

    if (rule) {
      await handleApiResponse(
        () =>
          SettingsService.updateInvoiceModuleAssignmentRule(rule.id, {
            ...payload,

            clearTaxBusinessEntity: !data.taxBusinessEntityId,

            clearFarm: !data.farmId,

            clearInvoiceDirection: !data.invoiceDirection,
          }),

        () => {
          toast.success("Reguła została zaktualizowana");

          handleClose();

          onSave();
        },

        undefined,

        "Błąd podczas aktualizacji reguły",
      );
    } else {
      await handleApiResponse(
        () => SettingsService.createInvoiceModuleAssignmentRule(payload),

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
          ? "Edytuj regułę przypisywania do modułu"
          : "Dodaj nową regułę przypisywania do modułu"}
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
                name="targetModule"
                control={control}
                rules={{ required: "Wybierz moduł docelowy" }}
                render={({ field }) => (
                  <TextField
                    {...field}
                    select
                    label="Moduł docelowy"
                    fullWidth
                    error={!!errors.targetModule}
                    helperText={errors.targetModule?.message}
                  >
                    {moduleTypeOptions.map((option) => (
                      <MenuItem key={option.value} value={option.value}>
                        {option.label}
                      </MenuItem>
                    ))}
                  </TextField>
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
              <Controller
                name="taxBusinessEntityId"
                control={control}
                render={({ field }) => (
                  <LoadingTextField
                    {...field}
                    select
                    label="Działalność (opcjonalna)"
                    fullWidth
                    loading={loadingTaxBusinessEntities}
                  >
                    <MenuItem value="">
                      <em>Wszystkie działalności</em>
                    </MenuItem>

                    {taxBusinessEntities.map((entity) => (
                      <MenuItem key={entity.id} value={entity.id}>
                        {entity.name} ({entity.businessType})
                      </MenuItem>
                    ))}
                  </LoadingTextField>
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
              <Controller
                name="farmId"
                control={control}
                render={({ field }) => (
                  <LoadingTextField
                    {...field}
                    select
                    label="Lokalizacja (opcjonalna)"
                    fullWidth
                    loading={loadingFarms}
                  >
                    <MenuItem value="">
                      <em>Wszystkie lokalizacje</em>
                    </MenuItem>

                    {farms.map((farm) => (
                      <MenuItem key={farm.id} value={farm.id}>
                        {farm.name}
                      </MenuItem>
                    ))}
                  </LoadingTextField>
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
              <Controller
                name="invoiceDirection"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    select
                    label="Kierunek faktury (opcjonalny)"
                    fullWidth
                  >
                    <MenuItem value="">
                      <em>Zakup i sprzedaż</em>
                    </MenuItem>

                    {invoiceDirectionOptions.map((option) => (
                      <MenuItem key={option.value} value={option.value}>
                        {option.label}
                      </MenuItem>
                    ))}
                  </TextField>
                )}
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <Typography variant="subtitle2" gutterBottom color="success.main">
                Słowa kluczowe (przynajmniej jedno musi wystąpić na fakturze)
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

              {includeKeywords.length === 0 && (
                <Typography
                  variant="caption"
                  color="text.secondary"
                  sx={{ mt: 0.5, display: "block" }}
                >
                  Np.: "pasza", "Wipasz", "starter", "grower", "gaz", "PGNiG"
                </Typography>
              )}
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

export default InvoiceModuleAssignmentRuleModal;
