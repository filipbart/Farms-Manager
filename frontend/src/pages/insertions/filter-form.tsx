import { Grid as MuiGrid, MenuItem, TextField } from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { Dayjs } from "dayjs";

import type {
  CycleDictModel,
  InsertionDictionary,
} from "../../models/insertions/insertion-dictionary";
import type { InsertionsFilterPaginationModel } from "../../models/insertions/insertions-filters";

interface FiltersFormProps {
  dictionary?: InsertionDictionary;
  filters: InsertionsFilterPaginationModel;
  dispatch: React.Dispatch<
    | { type: "set"; key: keyof InsertionsFilterPaginationModel; value: any }
    | { type: "setMultiple"; payload: Partial<InsertionsFilterPaginationModel> }
  >;
  uniqueCycles: CycleDictModel[];
}

const FiltersForm: React.FC<FiltersFormProps> = ({
  dictionary,
  filters,
  dispatch,
  uniqueCycles,
}) => {
  const handleMultiSelectChange = (
    key: keyof InsertionsFilterPaginationModel,
    values: string[]
  ) => {
    dispatch({ type: "set", key, value: values });
  };

  const handleDateChange = (
    key: "dateSince" | "dateTo",
    value: Dayjs | null
  ) => {
    dispatch({
      type: "set",
      key,
      value: value ? value.format("YYYY-MM-DD") : "",
    });
  };

  return (
    <MuiGrid container spacing={2}>
      {/*@ts-ignore*/}
      <MuiGrid item xs={12} sm={6} md={3}>
        <TextField
          label="Ferma"
          select
          slotProps={{ select: { multiple: true } }}
          sx={{ minWidth: 200 }}
          fullWidth
          value={filters.farmIds}
          onChange={(e) =>
            handleMultiSelectChange(
              "farmIds",
              Array.from(e.target.value as unknown as string[])
            )
          }
          disabled={!dictionary}
        >
          {dictionary?.farms.map((farm) => (
            <MenuItem key={farm.id} value={farm.id}>
              {farm.name}
            </MenuItem>
          )) || <MenuItem disabled>Ładowanie...</MenuItem>}
        </TextField>
      </MuiGrid>

      {/*@ts-ignore*/}
      <MuiGrid item xs={12} sm={6} md={3}>
        <TextField
          label="Identyfikator (cykl)"
          select
          slotProps={{ select: { multiple: true } }}
          sx={{ minWidth: 200 }}
          fullWidth
          value={filters.cycles.map((c) => `${c.identifier}-${c.year}`)}
          onChange={(e) => {
            const selectedValues = Array.from(
              e.target.value as unknown as string[]
            );
            const selectedCycles = uniqueCycles.filter((cycle) =>
              selectedValues.includes(`${cycle.identifier}-${cycle.year}`)
            );
            dispatch({ type: "set", key: "cycles", value: selectedCycles });
          }}
          disabled={!dictionary}
        >
          {uniqueCycles.map((cycle) => (
            <MenuItem
              key={`${cycle.identifier}-${cycle.year}`}
              value={`${cycle.identifier}-${cycle.year}`}
            >
              {cycle.identifier.toString()}/{cycle.year}
            </MenuItem>
          ))}
        </TextField>
      </MuiGrid>

      {/*@ts-ignore*/}
      <MuiGrid item xs={12} sm={6} md={3}>
        <TextField
          label="Kurnik"
          select
          slotProps={{ select: { multiple: true } }}
          fullWidth
          sx={{ minWidth: 200 }}
          value={filters.henhouseIds}
          onChange={(e) =>
            handleMultiSelectChange(
              "henhouseIds",
              Array.from(e.target.value as unknown as string[])
            )
          }
          disabled={filters.farmIds.length === 0 || !dictionary}
        >
          {filters.farmIds.length === 0 ? (
            <MenuItem disabled>Wybierz fermę najpierw</MenuItem>
          ) : (
            dictionary?.farms
              .filter((farm) => filters.farmIds.includes(farm.id))
              .flatMap((farm) =>
                farm.henhouses.map((henhouse) => (
                  <MenuItem key={henhouse.id} value={henhouse.id}>
                    {henhouse.name}
                  </MenuItem>
                ))
              )
          )}
        </TextField>
      </MuiGrid>

      {/*@ts-ignore*/}
      <MuiGrid item xs={12} sm={6} md={3}>
        <TextField
          label="Wylęgarnia"
          select
          slotProps={{ select: { multiple: true } }}
          sx={{ minWidth: 200 }}
          fullWidth
          value={filters.hatcheryIds}
          onChange={(e) =>
            handleMultiSelectChange(
              "hatcheryIds",
              Array.from(e.target.value as unknown as string[])
            )
          }
          disabled={!dictionary}
        >
          {dictionary?.hatcheries.map((hatchery) => (
            <MenuItem key={hatchery.id} value={hatchery.id}>
              {hatchery.name}
            </MenuItem>
          )) || <MenuItem disabled>Ładowanie...</MenuItem>}
        </TextField>
      </MuiGrid>

      {/*@ts-ignore*/}
      <MuiGrid item xs={12} sm={6} md={3}>
        <DatePicker
          label="Data od"
          value={filters.dateSince ? dayjs(filters.dateSince) : null}
          onChange={(val) => handleDateChange("dateSince", val)}
          slotProps={{
            textField: { fullWidth: true },
            actionBar: { actions: ["clear"] },
          }}
        />
      </MuiGrid>

      {/*@ts-ignore*/}
      <MuiGrid item xs={12} sm={6} md={3}>
        <DatePicker
          label="Data do"
          value={filters.dateTo ? dayjs(filters.dateTo) : null}
          onChange={(val) => handleDateChange("dateTo", val)}
          slotProps={{
            textField: { fullWidth: true },
            actionBar: { actions: ["clear"] },
          }}
        />
      </MuiGrid>
    </MuiGrid>
  );
};

export default FiltersForm;
