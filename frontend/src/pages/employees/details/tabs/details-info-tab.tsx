import { Box, Button, Grid, MenuItem, TextField } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import { Controller, useForm } from "react-hook-form";
import { useContext, useEffect } from "react";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import dayjs from "dayjs";
import { EmployeesService } from "../../../../services/employees-service";
import {
  EmployeeStatus,
  type UpdateEmployeeData,
} from "../../../../models/employees/employees";
import { useFarms } from "../../../../hooks/useFarms";
import LoadingTextField from "../../../../components/common/loading-textfield";
import { EmployeeContext } from "../../../../context/employee-context";
import { NotificationContext } from "../../../../context/notification-context";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";

const EmployeeInfoTab: React.FC = () => {
  const { employee, refetch, loading } = useContext(EmployeeContext);
  const { farms, loadingFarms, fetchFarms } = useFarms();
  const { refetch: refetchNotifications } = useContext(NotificationContext);

  const {
    control,
    handleSubmit,
    register,
    reset,
    watch,
    formState: { errors, isDirty },
  } = useForm<UpdateEmployeeData>({
    defaultValues: {
      fullName: "",
      farmId: "",
      position: "",
      status: EmployeeStatus.Active,
      contractType: "",
      salary: 0,
      startDate: "",
      endDate: undefined,
      comment: "",
    },
  });

  useEffect(() => {
    fetchFarms();
    if (employee) {
      reset({
        fullName: employee.fullName,
        farmId: employee.farmId,
        position: employee.position,
        status: employee.status,
        contractType: employee.contractType,
        salary: employee.salary,
        startDate: employee.startDate,
        endDate: employee.endDate ?? null,
        comment: employee.comment ?? "",
      });
    }
  }, [employee, reset]);

  const onSubmit = async (data: UpdateEmployeeData) => {
    if (!employee) return;

    console.log(data);
    try {
      await handleApiResponse(
        () => EmployeesService.updateEmployee(employee.id, data),
        () => {
          toast.success("Dane pracownika zostały zaktualizowane");
          refetch();
          refetchNotifications();
          reset(data);
        },
        (error) => {
          console.log(error);
        },
        "Nie udało się zaktualizować danych"
      );
    } catch {
      toast.error("Wystąpił błąd podczas aktualizacji");
    }
  };

  if (!employee) return null;

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} mt={2}>
      <Grid container spacing={2.5}>
        <Grid size={{ xs: 12, sm: 6 }}>
          <LoadingTextField
            label="Ferma"
            select
            fullWidth
            defaultValue={employee.farmId}
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
            label="Imię i nazwisko"
            fullWidth
            error={!!errors.fullName}
            helperText={errors.fullName?.message}
            {...register("fullName", { required: "To pole jest wymagane" })}
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Status"
            select
            fullWidth
            value={watch("status") || ""}
            defaultValue={employee.status}
            error={!!errors.status}
            helperText={errors.status?.message}
            {...register("status", { required: "Wybierz status" })}
          >
            {Object.entries(EmployeeStatus).map(([key, value]) => (
              <MenuItem key={key} value={value}>
                {value === EmployeeStatus.Active ? "Aktywny" : "Nieaktywny"}
              </MenuItem>
            ))}
          </TextField>
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Stanowisko"
            fullWidth
            error={!!errors.position}
            helperText={errors.position?.message}
            {...register("position", { required: "To pole jest wymagane" })}
          />
        </Grid>

        <Grid size={{ xs: 12, sm: 6 }}>
          <TextField
            label="Rodzaj umowy"
            select
            defaultValue={employee.contractType}
            fullWidth
            error={!!errors.contractType}
            helperText={errors.contractType?.message}
            {...register("contractType", { required: "Wybierz rodzaj umowy" })}
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
            error={!!errors.salary}
            helperText={errors.salary?.message}
            slotProps={{ htmlInput: { step: "0.01", min: 0 } }}
            {...register("salary", {
              required: "To pole jest wymagane",
              valueAsNumber: true,
              min: { value: 0, message: "Nie może być ujemne" },
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
                  field.onChange(date ? dayjs(date).format("YYYY-MM-DD") : "")
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
                label="Data zakończenia"
                format="DD.MM.YYYY"
                value={field.value ? dayjs(field.value) : null}
                onChange={(date) =>
                  field.onChange(date ? dayjs(date).format("YYYY-MM-DD") : null)
                }
                slotProps={{
                  textField: { fullWidth: true },
                  actionBar: { actions: ["clear", "cancel", "accept"] },
                }}
              />
            )}
          />
        </Grid>

        <Grid size={{ xs: 12 }}>
          <TextField
            label="Uwagi"
            multiline
            rows={3}
            fullWidth
            {...register("comment")}
          />
        </Grid>
      </Grid>

      <Box display="flex" justifyContent="flex-end" mt={3}>
        <Button
          type="submit"
          variant="contained"
          color="primary"
          startIcon={<MdSave />}
          disabled={!isDirty || loading}
        >
          Zapisz
        </Button>
      </Box>
    </Box>
  );
};

export default EmployeeInfoTab;
