import React, { useEffect, useState } from "react";
import {
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Grid,
  MenuItem,
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers";
import { toast } from "react-toastify";
import dayjs, { Dayjs } from "dayjs";
import { MdSave } from "react-icons/md";
import type { FeedPriceListModel } from "../../../../models/feeds/prices/feed-price";
import { FeedsService } from "../../../../services/feeds-service";
import { handleApiResponse } from "../../../../utils/axios/handle-api-response";
import { useFeedsNames } from "../../../../hooks/feeds/useFeedsNames";
import LoadingTextField from "../../../common/loading-textfield";
import LoadingButton from "../../../common/loading-button";
import AppDialog from "../../../common/app-dialog";
import { useFarms } from "../../../../hooks/useFarms";
import { FarmsService } from "../../../../services/farms-service";
import type CycleDto from "../../../../models/farms/latest-cycle";
import { TextField } from "@mui/material";

interface EditFeedPriceModalProps {
  open: boolean;
  onClose: () => void;
  onSave: () => void;
  feedPrice: FeedPriceListModel | null;
}

const EditFeedPriceModal: React.FC<EditFeedPriceModalProps> = ({
  open,
  onClose,
  onSave,
  feedPrice,
}) => {
  const [loading, setLoading] = useState(false);
  const { fetchFarms } = useFarms();
  const { feedsNames, loadingFeedsNames, fetchFeedsNames } = useFeedsNames();
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [loadingCycles, setLoadingCycles] = useState(false);

  const [form, setForm] = useState({
    priceDate: null as Dayjs | null,
    nameId: "",
    price: 0,
    farmId: "",
    cycleId: "",
  });

  const [errors, setErrors] = useState({
    priceDate: "",
    nameId: "",
    price: "",
    cycleId: "",
  });

  useEffect(() => {
    if (open) {
      fetchFarms();
      fetchFeedsNames();
    }
  }, [open, fetchFarms, fetchFeedsNames]);

  useEffect(() => {
    if (feedPrice) {
      const matchedFeed = feedsNames.find((f) => f.name === feedPrice.name);
      setForm({
        priceDate: dayjs(feedPrice.priceDate),
        nameId: matchedFeed?.id || "",
        price: feedPrice.price,
        farmId: feedPrice.farmId,
        cycleId: feedPrice.cycleId,
      });
    }
  }, [feedPrice, feedsNames]);

  useEffect(() => {
    const fetchCycles = async (farmId: string) => {
      if (!farmId) {
        setCycles([]);
        return;
      }
      setLoadingCycles(true);
      await handleApiResponse(
        () => FarmsService.getFarmCycles(farmId),
        (data) => setCycles(data.responseData ?? []),
        () => setCycles([]),
        "Nie udało się pobrać cykli dla wybranej fermy."
      );
      setLoadingCycles(false);
    };

    if (form.farmId) {
      fetchCycles(form.farmId);
    }
  }, [form.farmId]);

  const validate = () => {
    const e = { priceDate: "", nameId: "", price: "", cycleId: "" };

    if (!form.priceDate) e.priceDate = "Wymagana data publikacji";
    if (!form.nameId) e.nameId = "Wymagana nazwa paszy";
    if (!form.cycleId) e.cycleId = "Cykl jest wymagany";

    const qty = Number(form.price);
    if (isNaN(qty) || qty <= 0) e.price = "Cena musi być większa niż 0";

    setErrors(e);
    return Object.values(e).every((v) => !v);
  };

  const handleSave = async () => {
    if (!feedPrice?.id || !validate()) return;

    setLoading(true);
    await handleApiResponse(
      () =>
        FeedsService.updateFeedPrice(feedPrice.id, {
          priceDate: form.priceDate!.format("YYYY-MM-DD"),
          nameId: form.nameId,
          price: Number(form.price),
          cycleId: form.cycleId,
        }),
      () => {
        toast.success("Zaktualizowano cenę paszy");
        onSave();
        onClose();
      },
      undefined,
      "Nie udało się zapisać zmian"
    );
    setLoading(false);
  };

  return (
    <AppDialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Edytcja ceny paszy</DialogTitle>
      <DialogContent sx={{ pt: "20px !important" }}>
        <Grid container spacing={2}>
          <Grid size={12}>
            <TextField
              label="Ferma"
              value={feedPrice?.farmName || ""}
              fullWidth
              disabled
            />
          </Grid>
          <Grid size={12}>
            <LoadingTextField
              loading={loadingCycles}
              label="Cykl"
              select
              fullWidth
              disabled={loadingCycles || cycles.length === 0}
              value={form.cycleId}
              onChange={(e) =>
                setForm((f) => ({ ...f, cycleId: e.target.value }))
              }
              error={!!errors.cycleId}
              helperText={errors.cycleId}
            >
              {cycles.map((cycle) => (
                <MenuItem key={cycle.id} value={cycle.id}>
                  {`${cycle.identifier}/${cycle.year}`}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Grid>
          <Grid size={12}>
            <DatePicker
              label="Data publikacji"
              value={form.priceDate}
              onChange={(value) => setForm((f) => ({ ...f, priceDate: value }))}
              disableFuture
              format="DD.MM.YYYY"
              slotProps={{
                textField: {
                  error: !!errors.priceDate,
                  helperText: errors.priceDate,
                  fullWidth: true,
                },
              }}
            />
          </Grid>
          <Grid size={12}>
            <LoadingTextField
              select
              label="Nazwa paszy"
              value={form.nameId}
              onChange={(e) =>
                setForm((f) => ({ ...f, nameId: e.target.value }))
              }
              error={!!errors.nameId}
              helperText={errors.nameId}
              fullWidth
              loading={loadingFeedsNames}
            >
              {feedsNames.map((feed) => (
                <MenuItem key={feed.id} value={feed.id}>
                  {feed.name}
                </MenuItem>
              ))}
            </LoadingTextField>
          </Grid>
          <Grid size={12}>
            <TextField
              label="Cena [zł]"
              value={form.price}
              onChange={(e) =>
                setForm((f) => ({ ...f, price: Number(e.target.value) }))
              }
              error={!!errors.price}
              helperText={errors.price}
              fullWidth
              type="number"
            />
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Anuluj</Button>
        <LoadingButton
          loading={loading}
          onClick={handleSave}
          variant="contained"
          startIcon={<MdSave />}
        >
          Zapisz
        </LoadingButton>
      </DialogActions>
    </AppDialog>
  );
};

export default EditFeedPriceModal;
