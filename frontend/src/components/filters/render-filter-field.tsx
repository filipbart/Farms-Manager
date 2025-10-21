import { TextField, MenuItem, Autocomplete, FormControlLabel, Checkbox } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs, { Dayjs } from "dayjs";
import type { FilterConfig } from "./filter-types";

interface RenderFilterFieldProps {
  filter: FilterConfig;
  value: string[] | string | boolean | null;
  onChange: (val: string[] | string | boolean | null) => void;
}

export const RenderFilterField = ({
  filter,
  value,
  onChange,
}: RenderFilterFieldProps) => {
  if (filter.type === "multiSelect") {
    return (
      <TextField
        label={filter.label}
        select
        fullWidth
        sx={{ minWidth: 200 }}
        value={Array.isArray(value) ? value : []}
        onChange={(e) =>
          onChange(Array.from(e.target.value as unknown as string[]))
        }
        slotProps={{
          select: {
            multiple: true,

            renderValue: (selected) => {
              const selectedItems = selected as string[];
              if (selectedItems.length === 0) {
                return <em>Wybierz opcje...</em>;
              }
              if (selectedItems.length === 1) {
                return filter.options.find(
                  (opt) => opt.value === selectedItems[0]
                )?.label;
              }
              return `Wybrano: ${selectedItems.length}`;
            },
          },
        }}
        disabled={filter.disabled}
      >
        {filter.options.length > 0 ? (
          filter.options.map((opt) => (
            <MenuItem key={opt.value} value={opt.value}>
              {opt.label}
            </MenuItem>
          ))
        ) : (
          <MenuItem disabled>Brak opcji</MenuItem>
        )}
      </TextField>
    );
  }

  if (filter.type === "multiSelectSearch") {
    return (
      <Autocomplete
        multiple
        options={filter.options}
        getOptionLabel={(option) => option.label}
        value={filter.options.filter((opt) =>
          (value as string[])?.includes(opt.value)
        )}
        onChange={(_event, newValues) => {
          onChange(newValues.map((val) => val.value));
        }}
        disableCloseOnSelect
        disabled={filter.disabled}
        renderValue={(selectedOptions) => {
          if (selectedOptions.length === 0) {
            return null;
          }
          if (selectedOptions.length === 1) {
            return selectedOptions[0].label;
          }
          return `Wybrano: ${selectedOptions.length}`;
        }}
        sx={{ minWidth: 250 }}
        renderInput={(params) => <TextField {...params} label={filter.label} />}
      />
    );
  }

  if (filter.type === "select") {
    return (
      <TextField
        label={filter.label}
        select
        fullWidth
        sx={{ minWidth: 200 }}
        value={value || ""}
        onChange={(e) => onChange(e.target.value)}
        disabled={filter.disabled}
      >
        <MenuItem value="">— Brak wyboru —</MenuItem>
        {filter.options.length > 0 ? (
          filter.options.map((opt) => (
            <MenuItem key={opt.value} value={opt.value}>
              {opt.label}
            </MenuItem>
          ))
        ) : (
          <MenuItem disabled>Brak opcji</MenuItem>
        )}
      </TextField>
    );
  }

  if (filter.type === "date") {
    return (
      <DatePicker
        label={filter.label}
        value={value ? dayjs(value as string) : null}
        onChange={(val: Dayjs | null) =>
          onChange(val ? val.format("YYYY-MM-DD") : "")
        }
        sx={{ width: 200 }}
        slotProps={{
          textField: { fullWidth: false },
          actionBar: { actions: ["clear"] },
        }}
      />
    );
  }

  if (filter.type === "number") {
    return (
      <TextField
        label={filter.label}
        type="number"
        fullWidth
        sx={{ minWidth: 200 }}
        value={value || ""}
        onChange={(e) => onChange(e.target.value)}
        disabled={filter.disabled}
      />
    );
  }

  if (filter.type === "text") {
    return (
      <TextField
        label={filter.label}
        type="text"
        fullWidth
        sx={{ minWidth: 250 }}
        value={value || ""}
        onChange={(e) => onChange(e.target.value)}
        disabled={filter.disabled}
      />
    );
  }

  if (filter.type === "checkbox") {
    return (
      <FormControlLabel
        control={
          <Checkbox
            checked={Boolean(value)}
            onChange={(e) => onChange(e.target.checked)}
            disabled={filter.disabled}
          />
        }
        label={filter.label}
      />
    );
  }

  return null;
};
