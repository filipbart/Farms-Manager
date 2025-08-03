import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import { Controller, useForm } from "react-hook-form";
import { useState } from "react";
import dayjs from "dayjs";
import { toast } from "react-toastify";
import { EmployeesService } from "../../../services/employees-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import type { AddEmployeeReminderData } from "../../../models/employees/employees";
import LoadingButton from "../../common/loading-button";

interface AddEmployeeReminderModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
  employeeId: string;
}

const AddEmployeeReminderModal: React.FC<AddEmployeeReminderModalProps> = ({
  open,
  onClose,
  onSuccess,
  employeeId,
}) => {
  const [loading, setLoading] = useState(false);

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<AddEmployeeReminderData>({
    defaultValues: {
      title: "",
      dueDate: "",
      daysToRemind: 7,
    },
  });

  const handleClose = () => {
    reset();
    onClose();
  };

  const onSubmit = async (data: AddEmployeeReminderData) => {
    setLoading(true);
    await handleApiResponse(
      () => EmployeesService.addEmployeeReminder(employeeId, data),
      () => {
        toast.success("Przypomnienie zostało dodane");
        handleClose();
        onSuccess();
      },
      undefined,
      "Błąd podczas dodawania przypomnienia"
    );
    setLoading(false);
  };

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>Dodaj przypomnienie</DialogTitle>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogContent
          sx={{
            display: "flex",
            flexDirection: "column",
            gap: 2,
            pt: "8px !important",
          }}
        >
          <TextField
            label="Tytuł"
            fullWidth
            margin="normal"
            error={!!errors.title}
            helperText={errors.title?.message}
            {...register("title", {
              required: "Tytuł jest wymagany",
              minLength: { value: 3, message: "Minimum 3 znaki" },
            })}
          />
          <Controller
            name="dueDate"
            control={control}
            rules={{ required: "Data przypomnienia jest wymagana" }}
            render={({ field }) => (
              <DatePicker
                label="Data przypomnienia"
                format="DD.MM.YYYY"
                value={field.value ? dayjs(field.value) : null}
                onChange={(date) =>
                  field.onChange(date ? dayjs(date).format("YYYY-MM-DD") : "")
                }
                slotProps={{
                  textField: {
                    fullWidth: true,
                    margin: "normal",
                    error: !!errors.dueDate,
                    helperText: errors.dueDate?.message,
                  },
                }}
              />
            )}
          />
          <TextField
            label="Przypomnij X dni przed"
            type="number"
            fullWidth
            margin="normal"
            error={!!errors.daysToRemind}
            helperText={errors.daysToRemind?.message}
            {...register("daysToRemind", {
              required: "Liczba dni jest wymagana",
              valueAsNumber: true,
              min: { value: 0, message: "Wartość nie może być ujemna" },
            })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} variant="outlined" color="inherit">
            Anuluj
          </Button>
          <LoadingButton
            type="submit"
            variant="contained"
            disabled={loading}
            loading={loading}
          >
            Zapisz
          </LoadingButton>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default AddEmployeeReminderModal;
