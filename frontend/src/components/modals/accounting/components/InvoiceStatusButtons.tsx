import { Typography, Button, Box, ButtonGroup } from "@mui/material";
import CheckIcon from "@mui/icons-material/Check";
import PauseIcon from "@mui/icons-material/Pause";
import CloseIcon from "@mui/icons-material/Close";
import { KSeFInvoiceStatus } from "../../../../models/accounting/ksef-invoice";

interface InvoiceStatusButtonsProps {
  currentStatus: KSeFInvoiceStatus;
  saving: boolean;
  onAccept: () => void;
  onHold: () => void;
  onReject: () => void;
}

const InvoiceStatusButtons: React.FC<InvoiceStatusButtonsProps> = ({
  currentStatus,
  saving,
  onAccept,
  onHold,
  onReject,
}) => {
  return (
    <Box sx={{ mb: 2 }}>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
        Zmień status faktury:
      </Typography>
      <ButtonGroup variant="outlined" fullWidth>
        <Button
          color="success"
          variant={
            currentStatus === KSeFInvoiceStatus.Accepted
              ? "contained"
              : "outlined"
          }
          startIcon={<CheckIcon />}
          onClick={onAccept}
          disabled={saving || currentStatus === KSeFInvoiceStatus.Accepted}
        >
          Zaakceptuj
        </Button>
        <Button
          color="warning"
          variant="outlined"
          startIcon={<PauseIcon />}
          onClick={onHold}
          disabled={saving}
        >
          Wstrzymaj
        </Button>
        <Button
          color="error"
          variant={
            currentStatus === KSeFInvoiceStatus.Rejected
              ? "contained"
              : "outlined"
          }
          startIcon={<CloseIcon />}
          onClick={onReject}
          disabled={saving}
        >
          Odrzuć
        </Button>
      </ButtonGroup>
    </Box>
  );
};

export default InvoiceStatusButtons;
