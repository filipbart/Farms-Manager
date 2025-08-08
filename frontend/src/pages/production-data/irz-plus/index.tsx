import { Box, Grid, MenuItem, TextField, Typography } from "@mui/material";
import { useEffect, useMemo, useReducer, useState } from "react";
import { toast } from "react-toastify";
import type { CycleDictModel } from "../../../models/common/dictionaries";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import { FallenStockService } from "../../../services/production-data/fallen-stocks-service";
import {
  filterReducer,
  initialFilters,
  type FallenStocksDictionary,
} from "../../../models/fallen-stocks/fallen-stocks-filters";
import MainFallenStockPage from "./main";
import FallenStocksPickupPage from "./fallen-stock-pickup";

const ProductionDataIrzplusPage: React.FC = () => {
  const [filters, dispatch] = useReducer(filterReducer, initialFilters);
  const [dictionary, setDictionary] = useState<FallenStocksDictionary>();
  const [loadingDictionaries, setLoadingDictionaries] = useState(true);

  const [reloadMain, setReloadMain] = useState(0);

  const triggerReloadMain = () => {
    setReloadMain((prev) => prev + 1);
  };

  const uniqueCycles = useMemo(() => {
    if (!dictionary) return [];
    const map = new Map<string, CycleDictModel>();
    for (const cycle of dictionary.cycles) {
      const key = `${cycle.identifier}-${cycle.year}`;
      if (!map.has(key)) {
        map.set(key, cycle);
      }
    }
    return Array.from(map.values());
  }, [dictionary]);

  useEffect(() => {
    const fetchDictionaries = async () => {
      setLoadingDictionaries(true);
      try {
        await handleApiResponse(
          () => FallenStockService.getDictionaries(),
          (data) => setDictionary(data.responseData),
          undefined,
          "Błąd podczas pobierania słowników filtrów"
        );
      } catch {
        toast.error("Błąd podczas pobierania słowników filtrów");
      } finally {
        setLoadingDictionaries(false);
      }
    };
    fetchDictionaries();
  }, []);

  return (
    <Box p={4}>
      <Box
        mb={2}
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        gap={2}
      >
        <Typography variant="h4">Dane produkcyjne IRZplus</Typography>
      </Box>

      <Grid container spacing={2} sx={{ mb: 3 }}>
        <Grid size={{ xs: 12, sm: 4 }}>
          <TextField
            select
            label="Ferma"
            value={filters.farmId || ""}
            onChange={(e) =>
              dispatch({ type: "set", key: "farmId", value: e.target.value })
            }
            fullWidth
            disabled={loadingDictionaries}
          >
            <MenuItem value="">
              <em>--Brak wyboru--</em>
            </MenuItem>
            {dictionary?.farms.map((farm) => (
              <MenuItem key={farm.id} value={farm.id}>
                {farm.name}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
        <Grid size={{ xs: 12, sm: 2 }}>
          <TextField
            select
            label="Cykl"
            value={filters.cycle || ""}
            onChange={(e) =>
              dispatch({ type: "set", key: "cycle", value: e.target.value })
            }
            fullWidth
            disabled={loadingDictionaries}
          >
            <MenuItem value="">
              <em>--Brak wyboru--</em>
            </MenuItem>
            {uniqueCycles.map((cycle) => (
              <MenuItem
                key={cycle.id}
                value={`${cycle.identifier}-${cycle.year}`}
              >
                {`${cycle.identifier}/${cycle.year}`}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
      </Grid>

      <Grid container spacing={4} mt={2}>
        {/* Lewa kolumna */}
        <Grid size={{ xs: 12, lg: 8 }}>
          <MainFallenStockPage filters={filters} reloadTrigger={reloadMain} />
        </Grid>

        {/* Prawa kolumna */}
        <Grid size={{ xs: 12, lg: 4 }}>
          <FallenStocksPickupPage
            filters={filters}
            onReloadMain={triggerReloadMain}
          />
        </Grid>
      </Grid>
    </Box>
  );
};

export default ProductionDataIrzplusPage;
