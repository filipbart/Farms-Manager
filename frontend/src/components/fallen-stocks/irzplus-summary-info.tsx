import { Paper, Skeleton, Typography, Grid } from "@mui/material";
import {
  forwardRef,
  useCallback,
  useEffect,
  useImperativeHandle,
  useState,
} from "react";
import type { IrzSummaryData } from "../../models/fallen-stocks/fallen-stocks";
import { handleApiResponse } from "../../utils/axios/handle-api-response";
import { FallenStockService } from "../../services/production-data/fallen-stocks-service";

export interface IrzplusSummaryInfoRef {
  refetch: () => void;
}

interface IrzplusSummaryInfoProps {
  farmId: string | undefined;
  cycle: string | undefined;
}

const IrzplusSummaryInfo = forwardRef<
  IrzplusSummaryInfoRef,
  IrzplusSummaryInfoProps
>(({ farmId, cycle }, ref) => {
  const [data, setData] = useState<IrzSummaryData | null>(null);
  const [loading, setLoading] = useState(false);

  const fetchIrzData = useCallback(async () => {
    if (!farmId || !cycle) {
      setData(null);
      return;
    }
    setLoading(true);
    await handleApiResponse(
      () => FallenStockService.getIrzSummaryData(farmId, cycle),
      (data) => {
        if (data && data.responseData) {
          setData(data.responseData);
        }
      },
      (data) => {
        if (data && data.responseData) {
          setData(data.responseData);
        }
      },
      "Błąd podczas pobierania podsumowania z IRZplus."
    );
    setLoading(false);
  }, [farmId, cycle]);

  useImperativeHandle(ref, () => ({
    refetch: fetchIrzData,
  }));

  useEffect(() => {
    fetchIrzData();
  }, [fetchIrzData]);

  const renderRow = (label: string, value: number | null | undefined) => (
    <Grid
      container
      justifyContent="space-between"
      alignItems="center"
      sx={{ mb: 1 }}
    >
      <Grid>
        <Typography variant="body1">{label}</Typography>
      </Grid>
      <Grid>
        <Typography variant="body1" sx={{ fontWeight: 500 }}>
          {loading ? (
            <Skeleton width={60} />
          ) : value != null ? (
            formatNumber(value)
          ) : (
            "—"
          )}
        </Typography>
      </Grid>
    </Grid>
  );

  return (
    <Paper sx={{ p: 2, maxWidth: 600 }} variant="outlined">
      {renderRow("Aktualny stan stada w IRZplus:", data?.currentStockSize)}
      {renderRow(
        "Suma sztuk padłych zgłoszonych do IRZplus:",
        data?.reportedFallenStock
      )}
      {renderRow(
        "Suma sztuk padłych odebranych przez zakład utylizacyjny:",
        data?.collectedFallenStock
      )}
    </Paper>
  );
});

export default IrzplusSummaryInfo;

const formatNumber = (value: number | null | undefined): string => {
  const numberToFormat = value ?? 0;
  return new Intl.NumberFormat("pl-PL").format(numberToFormat);
};
