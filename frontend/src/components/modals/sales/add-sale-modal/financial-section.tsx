import React from "react";
import { Box, Typography, TextField, Button } from "@mui/material";
import type { SaleFormErrors, SaleFormState } from "./sale-form-types";

interface FinancialSectionProps {
  form: SaleFormState;
  dispatch: React.Dispatch<any>;
  errors: SaleFormErrors;
}

const FinancialSection: React.FC<FinancialSectionProps> = ({
  form,
  dispatch,
  errors,
}) => {
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
          dispatch({
            type: "SET_FIELD",
            name: "basePrice",
            value: parseFloat(e.target.value) || "",
          })
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
          dispatch({
            type: "SET_FIELD",
            name: "priceWithExtras",
            value: parseFloat(e.target.value) || "",
          })
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
          <TextField
            label="Nazwa"
            value={extra.name}
            onChange={(e) =>
              dispatch({
                type: "UPDATE_OTHER_EXTRA",
                index,
                field: "name",
                value: e.target.value,
              })
            }
            error={!!errors.otherExtras?.[index]?.name}
            helperText={errors.otherExtras?.[index]?.name}
            fullWidth
          />
          <TextField
            label="Wartość (zł)"
            type="number"
            value={extra.value}
            onChange={(e) =>
              dispatch({
                type: "UPDATE_OTHER_EXTRA",
                index,
                field: "value",
                value: parseFloat(e.target.value) || "",
              })
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
