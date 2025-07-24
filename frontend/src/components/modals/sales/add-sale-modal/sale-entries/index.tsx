import React from "react";
import {
  Grid,
  TextField,
  Box,
  Typography,
  MenuItem,
  Button,
  FormControl,
  FormHelperText,
  InputLabel,
  Select,
} from "@mui/material";
import type { SaleEntry } from "../../../../../models/sales/sales-entry";
import type { SaleEntriesSectionProps } from "./props";

const SaleEntriesSection: React.FC<SaleEntriesSectionProps> = ({
  entries,
  henhouses,
  errors,
  dispatch,
  setEntryErrors,
  farmId,
  saleFieldsExtra,
  entriesTableReady,
}) => {
  const handleFieldChange = (
    index: number,
    name: keyof SaleEntry,
    value: string
  ) => {
    dispatch({
      type: "UPDATE_ENTRY",
      index,
      name,
      value,
    });
    setEntryErrors(index, {
      ...(errors?.[index] || {}),
      [name]: undefined,
    });
  };

  const handleOtherExtraChange = (
    entryIndex: number,
    extraIndex: number,
    field: "name" | "value",
    value: string
  ) => {
    dispatch({
      type: "UPDATE_OTHER_EXTRA",
      entryIndex,
      extraIndex,
      field,
      value,
    });

    const entryErrors = errors?.[entryIndex] || {};
    const otherExtras = [...(entryErrors.otherExtras || [])];

    if (!otherExtras[extraIndex]) {
      otherExtras[extraIndex] = {};
    }

    otherExtras[extraIndex][field] = undefined;

    setEntryErrors(entryIndex, {
      ...entryErrors,
      otherExtras,
    });
  };

  return (
    <Box mt={3}>
      {entries.map((entry, index) => {
        if (entriesTableReady.includes(index)) {
          return null;
        }

        return (
          <Grid spacing={1} key={index} alignItems="flex-start" sx={{ mb: 2 }}>
            <Grid container spacing={2} mb={2}>
              <Grid container spacing={2} alignItems="center">
                <Grid sx={{ xs: 12, sm: 8 }}>
                  <Typography variant="h5">Dane produkcyjne</Typography>
                </Grid>
                <Grid sx={{ xs: 12, sm: 4 }} textAlign="right">
                  {index > 0 && (
                    <Button
                      variant="outlined"
                      color="error"
                      onClick={() => dispatch({ type: "REMOVE_ENTRY", index })}
                      fullWidth={true}
                    >
                      Usuń pozycję
                    </Button>
                  )}
                </Grid>
              </Grid>
              <Grid size={12} />
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField
                  select
                  label="Kurnik"
                  value={entry.henhouseId}
                  onChange={({ target }) => {
                    const selectedId = target.value;
                    const selectedHouse = henhouses.find(
                      (h) => h.id === selectedId
                    );

                    handleFieldChange(index, "henhouseId", selectedId);
                    handleFieldChange(
                      index,
                      "henhouseName",
                      selectedHouse ? selectedHouse.name : ""
                    );
                  }}
                  fullWidth
                  disabled={!farmId}
                  error={!!errors?.[index]?.henhouseId}
                  helperText={errors?.[index]?.henhouseId}
                >
                  {henhouses.map((house) => (
                    <MenuItem key={house.id} value={house.id}>
                      {house.name}
                    </MenuItem>
                  ))}
                </TextField>
              </Grid>
              <Grid size={6}></Grid>
              <Grid size={12}>
                <Typography variant="h6" mt={2} mb={1}>
                  Przyjęto
                </Typography>
              </Grid>

              <Grid size={{ xs: 12, sm: 3 }}>
                <TextField
                  label="Waga (kg)"
                  type="number"
                  value={entry.weight}
                  onChange={(e) =>
                    handleFieldChange(index, "weight", e.target.value)
                  }
                  slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                  error={!!errors?.[index]?.weight}
                  helperText={errors?.[index]?.weight}
                  fullWidth
                />
              </Grid>

              <Grid size={{ xs: 12, sm: 3 }}>
                <TextField
                  label="Sztuki"
                  type="number"
                  value={entry.quantity}
                  onChange={(e) =>
                    handleFieldChange(index, "quantity", e.target.value)
                  }
                  slotProps={{ htmlInput: { min: 1, step: 1 } }}
                  error={!!errors?.[index]?.quantity}
                  helperText={errors?.[index]?.quantity}
                  fullWidth
                />
              </Grid>
            </Grid>
            <Grid container spacing={2}>
              <Grid size={12}>
                <Typography variant="h6" mt={2} mb={1}>
                  Konfiskata
                </Typography>
              </Grid>
              <Grid size={{ xs: 12, sm: 3 }}>
                <TextField
                  label="Waga (kg)"
                  type="number"
                  value={entry.confiscatedWeight}
                  onChange={(e) =>
                    handleFieldChange(
                      index,
                      "confiscatedWeight",
                      e.target.value
                    )
                  }
                  slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                  error={!!errors?.[index]?.confiscatedWeight}
                  helperText={errors?.[index]?.confiscatedWeight}
                  fullWidth
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 3 }}>
                <TextField
                  label="Sztuki"
                  type="number"
                  value={entry.confiscatedCount}
                  onChange={(e) =>
                    handleFieldChange(index, "confiscatedCount", e.target.value)
                  }
                  slotProps={{ htmlInput: { min: 0, step: 1 } }}
                  error={!!errors?.[index]?.confiscatedCount}
                  helperText={errors?.[index]?.confiscatedCount}
                  fullWidth
                />
              </Grid>

              <Grid size={12}>
                <Typography variant="h6" mt={2} mb={1}>
                  Martwe
                </Typography>
              </Grid>

              <Grid size={{ xs: 12, sm: 3 }}>
                <TextField
                  label="Waga (kg)"
                  type="number"
                  value={entry.deadWeight}
                  onChange={(e) =>
                    handleFieldChange(index, "deadWeight", e.target.value)
                  }
                  slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                  error={!!errors?.[index]?.deadWeight}
                  helperText={errors?.[index]?.deadWeight}
                  fullWidth
                />
              </Grid>

              <Grid size={{ xs: 12, sm: 3 }}>
                <TextField
                  label="Sztuki"
                  type="number"
                  value={entry.deadCount}
                  onChange={(e) =>
                    handleFieldChange(index, "deadCount", e.target.value)
                  }
                  slotProps={{ htmlInput: { min: 0, step: 1 } }}
                  error={!!errors?.[index]?.deadCount}
                  helperText={errors?.[index]?.deadCount}
                  fullWidth
                />
              </Grid>

              <Grid size={6}></Grid>

              <Grid size={{ xs: 12, sm: 6 }} mt={5}>
                <TextField
                  label="Waga hodowcy (kg)"
                  type="number"
                  value={entry.farmerWeight}
                  onChange={(e) =>
                    handleFieldChange(index, "farmerWeight", e.target.value)
                  }
                  slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                  error={!!errors?.[index]?.farmerWeight}
                  helperText={errors?.[index]?.farmerWeight}
                  fullWidth
                />
              </Grid>
            </Grid>
            <Grid container spacing={2} mb={2}>
              <Grid size={12}>
                <Typography variant="h5" mt={2} mb={1}>
                  Dane finansowe
                </Typography>
              </Grid>

              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField
                  label="Cena bazowa (zł)"
                  type="number"
                  value={entry.basePrice}
                  onChange={(e) =>
                    handleFieldChange(index, "basePrice", e.target.value)
                  }
                  error={!!errors?.[index]?.basePrice}
                  helperText={errors?.[index]?.basePrice}
                  slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                  fullWidth
                  sx={{ mb: 2 }}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField
                  label="Cena z dodatkami (zł)"
                  type="number"
                  value={entry.priceWithExtras}
                  onChange={(e) =>
                    handleFieldChange(index, "priceWithExtras", e.target.value)
                  }
                  error={!!errors?.[index]?.priceWithExtras}
                  helperText={errors?.[index]?.priceWithExtras}
                  slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                  fullWidth
                  sx={{ mb: 2 }}
                />
              </Grid>
            </Grid>

            <Grid container spacing={2} mb={2}>
              <Grid size={12}>
                <Typography variant="subtitle1" mt={2} mb={1}>
                  Inne dodatki
                </Typography>
              </Grid>

              {entry.otherExtras?.map((extra, extraIndex) => (
                <Grid size={12} key={extraIndex}>
                  <Box display="flex" gap={2} alignItems="center" mb={1}>
                    <FormControl
                      fullWidth
                      error={!!errors?.[index]?.otherExtras?.[extraIndex]?.name}
                      sx={{ minWidth: 150 }}
                    >
                      <InputLabel
                        id={`extra-name-label-${index}-${extraIndex}`}
                      >
                        Nazwa
                      </InputLabel>
                      <Select
                        labelId={`extra-name-label-${index}-${extraIndex}`}
                        value={extra.name}
                        label="Nazwa"
                        onChange={(e) =>
                          handleOtherExtraChange(
                            index,
                            extraIndex,
                            "name",
                            e.target.value
                          )
                        }
                      >
                        {saleFieldsExtra.map((field) => (
                          <MenuItem key={field.id} value={field.name}>
                            {field.name}
                          </MenuItem>
                        ))}
                      </Select>
                      {!!errors?.[index]?.otherExtras?.[extraIndex]?.name && (
                        <FormHelperText>
                          {errors[index].otherExtras[extraIndex].name}
                        </FormHelperText>
                      )}
                    </FormControl>

                    <TextField
                      label="Wartość (zł)"
                      type="number"
                      value={extra.value}
                      onChange={(e) =>
                        handleOtherExtraChange(
                          index,
                          extraIndex,
                          "value",
                          e.target.value
                        )
                      }
                      error={
                        !!errors?.[index]?.otherExtras?.[extraIndex]?.value
                      }
                      helperText={
                        errors?.[index]?.otherExtras?.[extraIndex]?.value
                      }
                      inputProps={{ min: 0, step: 0.01 }}
                      fullWidth
                    />

                    <Button
                      onClick={() =>
                        dispatch({
                          type: "REMOVE_OTHER_EXTRA",
                          entryIndex: index,
                          extraIndex,
                        })
                      }
                      color="error"
                    >
                      Usuń
                    </Button>
                  </Box>
                </Grid>
              ))}

              <Grid size={12}>
                <Button
                  variant="outlined"
                  onClick={() =>
                    dispatch({ type: "ADD_OTHER_EXTRA", entryIndex: index })
                  }
                >
                  Dodaj dodatek
                </Button>
              </Grid>
            </Grid>

            <Grid size={{ xs: 12, sm: 6 }} mt={5}>
              <TextField
                label="Komentarz"
                multiline
                value={entry.comment}
                onChange={(e) =>
                  handleFieldChange(index, "comment", e.target.value)
                }
                fullWidth
                sx={{ mb: 2 }}
              />
            </Grid>

            <Grid size={{ xs: 12, sm: 3 }} textAlign="center"></Grid>
          </Grid>
        );
      })}
    </Box>
  );
};

export default SaleEntriesSection;
