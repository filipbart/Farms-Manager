import React from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  FormControl,
  FormHelperText,
  InputLabel,
  MenuItem,
  Select,
} from "@mui/material";
import type { SaleFormErrors, SaleFormState } from "./sale-form-types";
import type { SaleFieldsExtraRow } from "../../../../services/sales-settings-service";

interface FinancialSectionProps {
  form: SaleFormState;
  saleFieldsExtra: SaleFieldsExtraRow[];
  dispatch: React.Dispatch<any>;
  errors: SaleFormErrors;
  setErrors: React.Dispatch<React.SetStateAction<SaleFormErrors>>;
}

const FinancialSection: React.FC<FinancialSectionProps> = ({
  form,
  saleFieldsExtra,
  dispatch,
  errors,
  setErrors,
}) => {
  const handleFieldChange = (name: keyof SaleFormState, value: any) => {
    dispatch({
      type: "SET_FIELD",
      name,
      value,
    });

    setErrors((prevErrors) => ({
      ...prevErrors,
      [name]: undefined,
    }));
  };

  const handleOtherExtraChange = (
    index: number,
    field: keyof (typeof form.otherExtras)[number],
    value: any
  ) => {
    dispatch({
      type: "UPDATE_OTHER_EXTRA",
      index,
      field,
      value,
    });

    setErrors((prevErrors) => {
      const newErrors = { ...prevErrors };
      if (!newErrors.otherExtras) newErrors.otherExtras = [];
      newErrors.otherExtras = [...newErrors.otherExtras];
      newErrors.otherExtras[index] = {
        ...(newErrors.otherExtras[index] || {}),
        [field]: undefined,
      };
      return newErrors;
    });
  };

  return (
    <Box>
      <Typography variant="h6" mt={2} mb={1}>
        Dane finansowe
      </Typography>

      <TextField
        label="Cena bazowa (zł)"
        type="number"
        value={form.basePrice}
        onChange={(e) =>
          handleFieldChange("basePrice", parseFloat(e.target.value) || "")
        }
        error={!!errors.basePrice}
        helperText={errors.basePrice}
        slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
        fullWidth
        sx={{ mb: 2 }}
      />

      <TextField
        label="Cena z dodatkami (zł)"
        type="number"
        value={form.priceWithExtras}
        onChange={(e) =>
          handleFieldChange("priceWithExtras", parseFloat(e.target.value) || "")
        }
        error={!!errors.priceWithExtras}
        helperText={errors.priceWithExtras}
        slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
        fullWidth
        sx={{ mb: 2 }}
      />

      <TextField
        label="Komentarz"
        multiline
        value={form.comment}
        onChange={(e) =>
          dispatch({
            type: "SET_FIELD",
            name: "comment",
            value: e.target.value,
          })
        }
        fullWidth
        sx={{ mb: 2 }}
      />

      <Typography variant="subtitle1" mt={2} mb={1}>
        Inne dodatki
      </Typography>

      {form.otherExtras.map((extra, index) => (
        <Box key={index} display="flex" gap={2} alignItems="center" mb={1}>
          <FormControl
            fullWidth
            error={!!errors.otherExtras?.[index]?.name}
            sx={{ minWidth: 150 }}
          >
            <InputLabel id={`extra-name-label-${index}`}>Nazwa</InputLabel>
            <Select
              labelId={`extra-name-label-${index}`}
              value={extra.name}
              label="Nazwa"
              onChange={(e) =>
                handleOtherExtraChange(index, "name", e.target.value)
              }
            >
              {saleFieldsExtra.map((field) => (
                <MenuItem key={field.id} value={field.name}>
                  {field.name}
                </MenuItem>
              ))}
            </Select>
            {!!errors.otherExtras?.[index]?.name && (
              <FormHelperText>{errors.otherExtras[index].name}</FormHelperText>
            )}
          </FormControl>
          <TextField
            label="Wartość (zł)"
            type="number"
            value={extra.value}
            onChange={(e) =>
              handleOtherExtraChange(index, "value", e.target.value)
            }
            error={!!errors.otherExtras?.[index]?.value}
            helperText={errors.otherExtras?.[index]?.value}
            slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
            fullWidth
          />
          <Button
            onClick={() => dispatch({ type: "REMOVE_OTHER_EXTRA", index })}
            color="error"
          >
            Usuń
          </Button>
        </Box>
      ))}

      <Button
        variant="outlined"
        onClick={() => dispatch({ type: "ADD_OTHER_EXTRA" })}
      >
        Dodaj dodatek
      </Button>
    </Box>
  );
};

export default FinancialSection;
