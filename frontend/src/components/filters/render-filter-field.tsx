import { TextField, MenuItem } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs, { Dayjs } from "dayjs";
import type { FilterConfig } from "./filter-types";

interface RenderFilterFieldProps {
  filter: FilterConfig;
  value: string[] | string | null;
  onChange: (val: string[] | string | null) => void;
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
        slotProps={{ select: { multiple: true } }}
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

  if (filter.type === "date") {
    return (
      <DatePicker
        label={filter.label}
        value={value ? dayjs(value as string) : null}
        onChange={(val: Dayjs | null) =>
          onChange(val ? val.format("YYYY-MM-DD") : "")
        }
        slotProps={{
          textField: { fullWidth: true },
          actionBar: { actions: ["clear"] },
        }}
      />
    );
  }

  return null;
};
