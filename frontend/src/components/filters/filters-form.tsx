import React from "react";
import { Grid } from "@mui/material";
import type { FilterConfig } from "./filter-types";
import { RenderFilterField } from "./render-filter-field";

export interface FiltersFormProps<T extends Record<string, any>> {
  config: FilterConfig<Extract<keyof T, string>>[];
  filters: T;
  dispatch: React.Dispatch<
    | { type: "set"; key: keyof T; value: any }
    | { type: "setMultiple"; payload: Partial<T> }
  >;
}

function FiltersForm<T extends Record<string, any>>({
  config,
  filters,
  dispatch,
}: FiltersFormProps<T>) {
  return (
    <Grid container spacing={2}>
      {config.map((filter) => {
        const key = filter.key as keyof T;
        const gridSize = filter.gridSize || { xs: 12, sm: 6, md: 4, lg: 3 };
        return (
          <Grid key={key as string} size={gridSize}>
            <RenderFilterField
              filter={filter}
              value={filters[key]}
              onChange={(val) =>
                dispatch({
                  type: "setMultiple",
                  payload: { [key]: val } as Partial<T>,
                })
              }
            />
          </Grid>
        );
      })}
    </Grid>
  );
}

export default FiltersForm;
