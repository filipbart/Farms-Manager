import React, { useEffect, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  TextField,
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
  const { feedsNames, loadingFeedsNames, fetchFeedsNames } = useFeedsNames();
  const [form, setForm] = useState({
    priceDate: null as Dayjs | null,
    nameId: "",
    price: 0,
  });

  const [errors, setErrors] = useState({
    priceDate: "",
    nameId: "",
    price: "",
  });

  useEffect(() => {
    if (open && feedPrice) {
      fetchFeedsNames();
    }
  }, [open, feedPrice]);

  useEffect(() => {
    if (feedPrice && feedsNames.length > 0) {
      const matchedFeed = feedsNames.find((f) => f.name === feedPrice.name);
      setForm({
        priceDate: dayjs(feedPrice.priceDate),
        nameId: matchedFeed?.id || "",
        price: feedPrice.price,
      });
    }
  }, [feedPrice, feedsNames]);

  const validate = () => {
    const e = { priceDate: "", nameId: "", price: "" };

    if (!form.priceDate) e.priceDate = "Wymagana data publikacji";
    if (!form.nameId) e.nameId = "Wymagana nazwa paszy";

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
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>Edytcja ceny paszy</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" gap={2} mt={1}>
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

          <LoadingTextField
            select
            label="Nazwa paszy"
            value={form.nameId}
            onChange={(e) => setForm((f) => ({ ...f, nameId: e.target.value }))}
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
        </Box>
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
    </Dialog>
  );
};

export default EditFeedPriceModal;
