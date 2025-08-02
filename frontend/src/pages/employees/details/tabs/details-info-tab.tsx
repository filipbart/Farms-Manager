import { Box, Button, Grid, MenuItem, TextField } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import { Controller, useForm } from "react-hook-form";
import { useContext, useEffect } from "react";
import { MdSave } from "react-icons/md";
import { toast } from "react-toastify";
import dayjs from "dayjs";
import { EmployeeContext } from "..";
import { EmployeesService } from "../../../../services/employees-service";

const EmployeeInfoTab: React.FC = () => {
  const { employee, setEmployee, refetch, loading } =
    useContext(EmployeeContext);

  const {
    control,
    handleSubmit,
    register,
    reset,
    formState: { errors, isDirty },
  } = useForm({
    defaultValues: {
      fullName: "",
      farmId: "",
      position: "",
      contractType: "",
      salary: 0,
      startDate: "",
      endDate: "",
      comment: "",
    },
  });

  useEffect(() => {
    if (employee) {
      reset({
        fullName: employee.fullName,
        farmId: employee.farmId,
        position: employee.position,
        contractType: employee.contractType,
        salary: employee.salary,
        startDate: employee.startDate,
        endDate: employee.endDate ?? "",
        comment: employee.comment ?? "",
      });
    }
  }, [employee, reset]);

  const onSubmit = async (data: any) => {
    if (!employee) return;

    const updated = { ...employee, ...data };

    try {
      const response = await EmployeesService.updateEmployee(
        employee.id,
        updated
      );
      if (response.success) {
        toast.success("Dane pracownika zostały zaktualizowane");
        setEmployee(response.responseData);
        refetch();
        reset(data);
      } else {
        toast.error("Nie udało się zaktualizować danych");
      }
    } catch (err) {
      console.error("Błąd aktualizacji:", err);
      toast.error("Wystąpił błąd podczas aktualizacji");
    }
  };

  if (!employee) return null;

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} mt={2}>
      <Grid container spacing={2.5}>
        <Grid size={{ xs: 12 }}>
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
                label="Data zakończenia (opcjonalnie)"
                format="DD.MM.YYYY"
                value={field.value ? dayjs(field.value) : null}
                onChange={(date) =>
                  field.onChange(date ? dayjs(date).format("YYYY-MM-DD") : "")
                }
                slotProps={{ textField: { fullWidth: true } }}
              />
            )}
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
