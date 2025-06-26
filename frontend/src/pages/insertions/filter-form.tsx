import { Grid, MenuItem, TextField } from "@mui/material";
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
    dispatch({
      type: "setMultiple",
      payload: {
        [key]: values,
      },
    });
  };

  const handleDateChange = (
    key: "dateSince" | "dateTo",
    value: Dayjs | null
  ) => {
    dispatch({
      type: "setMultiple",
      payload: {
        [key]: value ? value.format("YYYY-MM-DD") : "",
      },
    });
  };

  return (
    <>
      <Grid container spacing={2}>
        <Grid>
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
        </Grid>

        <Grid>
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
        </Grid>

        <Grid>
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
        </Grid>

        <Grid>
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
        </Grid>
      </Grid>
      <Grid container spacing={2} mt={2}>
        <Grid>
          <DatePicker
            label="Data od"
            value={filters.dateSince ? dayjs(filters.dateSince) : null}
            onChange={(val) => handleDateChange("dateSince", val)}
            slotProps={{
              textField: { fullWidth: true },
              actionBar: { actions: ["clear"] },
            }}
          />
        </Grid>

        <Grid>
          <DatePicker
            label="Data do"
            value={filters.dateTo ? dayjs(filters.dateTo) : null}
            onChange={(val) => handleDateChange("dateTo", val)}
            slotProps={{
              textField: { fullWidth: true },
              actionBar: { actions: ["clear"] },
            }}
          />
        </Grid>
      </Grid>
    </>
  );
};

export default FiltersForm;
