import {
  DialogTitle,
  DialogContent,
  Grid,
  DialogActions,
  TextField,
  MenuItem,
  Button,
  FormControlLabel,
  Checkbox,
} from "@mui/material";
import { useState, useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { MdSave } from "react-icons/md";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { useFarms } from "../../../hooks/useFarms";
import type { AddEmployeeData } from "../../../models/employees/employees";
import { EmployeesService } from "../../../services/employees-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import AppDialog from "../../common/app-dialog";
import LoadingButton from "../../common/loading-button";
import LoadingTextField from "../../common/loading-textfield";

interface AddEmployeeModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
}

const AddEmployeeModal: React.FC<AddEmployeeModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const [loading, setLoading] = useState(false);
  const { farms, loadingFarms, fetchFarms } = useFarms();

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<AddEmployeeData>();

  useEffect(() => {
    if (open) {
      fetchFarms();
    }
  }, [open, fetchFarms]);

  const handleClose = () => {
    reset();
    onClose();
  };

  const handleSave = async (data: AddEmployeeData) => {
    setLoading(true);
    await handleApiResponse(
      () => EmployeesService.addEmployee(data),
      () => {
        toast.success("Pomyślnie dodano pracownika");
        handleClose();
        onSave();
      },
      undefined,
      "Wystąpił błąd podczas dodawania pracownika"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={handleClose} fullWidth maxWidth="md">
      <DialogTitle>Dodaj nowego pracownika</DialogTitle>
      <form onSubmit={handleSubmit(handleSave)}>
        <DialogContent>
          <Grid container spacing={2.5} sx={{ mt: 0.5 }}>
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Imię i nazwisko"
                fullWidth
                error={!!errors.fullName}
                helperText={errors.fullName?.message}
                {...register("fullName", {
                  required: "Imię i nazwisko jest wymagane",
                })}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <LoadingTextField
                label="Ferma"
                select
                fullWidth
                loading={loadingFarms}
                error={!!errors.farmId}
                helperText={errors.farmId?.message}
                {...register("farmId", {
                  required: "Ferma jest wymagana",
                })}
              >
                {farms.map((farm) => (
                  <MenuItem key={farm.id} value={farm.id}>
                    {farm.name}
                  </MenuItem>
                ))}
              </LoadingTextField>
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Stanowisko"
                fullWidth
                error={!!errors.position}
                helperText={errors.position?.message}
                {...register("position", {
                  required: "Stanowisko jest wymagane",
                })}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Rodzaj umowy"
                select
                fullWidth
                defaultValue=""
                error={!!errors.contractType}
                helperText={errors.contractType?.message}
                {...register("contractType", {
                  required: "Rodzaj umowy jest wymagany",
                })}
              >
                <MenuItem value="Umowa o pracę">Umowa o pracę</MenuItem>
                <MenuItem value="Umowa zlecenie">Umowa zlecenie</MenuItem>
                <MenuItem value="B2B">B2B</MenuItem>
                <MenuItem value="Inna">Inna</MenuItem>
              </TextField>
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <TextField
                label="Wynagrodzenie [zł]"
                type="number"
                fullWidth
                slotProps={{ htmlInput: { step: "0.01", min: 0 } }}
                error={!!errors.salary}
                helperText={errors.salary?.message}
                {...register("salary", {
                  required: "Wynagrodzenie jest wymagane",
                  valueAsNumber: true,
                  min: { value: 0, message: "Wartość nie może być ujemna" },
                })}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <Controller
                name="startDate"
                control={control}
                rules={{ required: "Data rozpoczęcia jest wymagana" }}
                render={({ field }) => (
                  <DatePicker
                    label="Data rozpoczęcia"
                    format="DD.MM.YYYY"
                    value={field.value ? dayjs(field.value) : null}
                    onChange={(date) =>
                      field.onChange(
                        date ? dayjs(date).format("YYYY-MM-DD") : ""
                      )
                    }
                    slotProps={{
                      textField: {
                        fullWidth: true,
                        error: !!errors.startDate,
                        helperText: errors.startDate?.message,
                      },
                    }}
                  />
                )}
              />
            </Grid>
            <Grid size={{ xs: 12, sm: 6 }}>
              <Controller
                name="endDate"
                control={control}
                render={({ field }) => (
                  <DatePicker
                    label="Data zakończenia (opcjonalnie)"
                    format="DD.MM.YYYY"
                    value={field.value ? dayjs(field.value) : null}
                    onChange={(date) =>
                      field.onChange(
                        date ? dayjs(date).format("YYYY-MM-DD") : ""
                      )
                    }
                    slotProps={{ textField: { fullWidth: true } }}
                  />
                )}
              />
            </Grid>
            <Grid size={{ xs: 12 }}>
              <FormControlLabel
                control={
                  <Controller
                    name="addToAdvances"
                    control={control}
                    render={({ field }) => (
                      <Checkbox
                        {...field}
                        checked={!!field.value}
                        onChange={(e) => field.onChange(e.target.checked)}
                      />
                    )}
                  />
                }
                label="Dodaj do ewidencji zaliczek"
              />
            </Grid>
            <Grid size={{ xs: 12 }}>
              <TextField
                label="Komentarz"
                multiline
                rows={3}
                fullWidth
                {...register("comment")}
              />
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
            Zapisz
          </LoadingButton>
        </DialogActions>
      </form>
    </AppDialog>
  );
};

export default AddEmployeeModal;
