import React, { useEffect, useState } from "react";
import {
  Box,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Button,
  CircularProgress,
  Typography,
  Autocomplete,
} from "@mui/material";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import {
  AccountingService,
  type CreateModuleEntityRequest,
  type AcceptInvoiceRequest,
} from "../../../services/accounting-service";
import { FarmsService } from "../../../services/farms-service";
import { FeedsService } from "../../../services/feeds-service";
import { GasService } from "../../../services/gas-service";
import { ExpensesService } from "../../../services/expenses-service";
import { SlaughterhousesService } from "../../../services/slaughterhouses-service";
import { handleApiResponse } from "../../../utils/axios/handle-api-response";
import {
  ModuleType,
  type KSeFLineItem,
} from "../../../models/accounting/ksef-invoice";
import type FarmRowModel from "../../../models/farms/farm-row-model";
import type CycleDto from "../../../models/farms/latest-cycle";
import type { GasContractorRow } from "../../../models/gas/gas-contractors";
import type { ExpenseContractorRow } from "../../../models/expenses/expenses-contractors";
import type { SlaughterhouseRowModel } from "../../../models/slaughterhouses/slaughterhouse-row-model";
import type { FeedsNamesRow } from "../../../models/feeds/feeds-names";
import type { ExpenseTypeRow } from "../../../models/expenses/expenses-types";

interface ModuleEntityFormProps {
  invoiceId: string;
  moduleType: string;
  invoiceData: {
    invoiceNumber: string;
    invoiceDate: string;
    dueDate?: string;
    sellerName: string;
    sellerNip: string;
    buyerName: string;
    buyerNip: string;
    grossAmount: number;
    netAmount: number;
    vatAmount: number;
    lineItems?: KSeFLineItem[];
  };
  farms: FarmRowModel[];
  selectedFarmId?: string;
  selectedCycleId?: string;
  onSuccess: () => void;
  onCancel: () => void;
  /**
   * Mode: 'create' - creates module entity without changing status
   * Mode: 'accept' - creates module entity AND changes status to Accepted
   */
  mode?: "create" | "accept";
}

interface FeedFormData {
  farmId: string;
  cycleId: string;
  henhouseId: string;
  invoiceNumber: string;
  bankAccountNumber: string;
  vendorName: string;
  itemName: string;
  quantity: number;
  unitPrice: number;
  invoiceDate: string;
  dueDate: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  comment: string;
}

interface GasFormData {
  farmId: string;
  contractorId: string;
  invoiceNumber: string;
  invoiceDate: string;
  invoiceTotal: number;
  unitPrice: number;
  quantity: number;
  comment: string;
}

interface ExpenseFormData {
  farmId: string;
  cycleId: string;
  expenseContractorId: string;
  expenseTypeId: string;
  invoiceNumber: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
  invoiceDate: string;
  comment: string;
}

interface SaleFormData {
  farmId: string;
  cycleId: string;
  slaughterhouseId: string;
  invoiceNumber: string;
  invoiceDate: string;
  dueDate: string;
  invoiceTotal: number;
  subTotal: number;
  vatAmount: number;
}

const ModuleEntityForm: React.FC<ModuleEntityFormProps> = ({
  invoiceId,
  moduleType,
  invoiceData,
  farms,
  selectedFarmId,
  selectedCycleId,
  onSuccess,
  onCancel,
  mode = "create",
}) => {
  const [loading, setLoading] = useState(false);
  const [cycles, setCycles] = useState<CycleDto[]>([]);
  const [henhouses, setHenhouses] = useState<{ id: string; name: string }[]>(
    []
  );
  const [feedNames, setFeedNames] = useState<FeedsNamesRow[]>([]);
  const [gasContractors, setGasContractors] = useState<GasContractorRow[]>([]);
  const [expenseContractors, setExpenseContractors] = useState<
    ExpenseContractorRow[]
  >([]);
  const [slaughterhouses, setSlaughterhouses] = useState<
    SlaughterhouseRowModel[]
  >([]);
  const [expenseTypes, setExpenseTypes] = useState<ExpenseTypeRow[]>([]);

  // Feed form
  const feedForm = useForm<FeedFormData>({
    defaultValues: {
      farmId: selectedFarmId || "",
      cycleId: selectedCycleId || "",
      henhouseId: "",
      invoiceNumber: invoiceData.invoiceNumber,
      bankAccountNumber: "",
      vendorName: invoiceData.sellerName,
      itemName: "",
      quantity: 0,
      unitPrice: 0,
      invoiceDate: invoiceData.invoiceDate,
      dueDate: invoiceData.dueDate || "",
      invoiceTotal: invoiceData.grossAmount,
      subTotal: invoiceData.netAmount,
      vatAmount: invoiceData.vatAmount,
      comment: "",
    },
  });

  // Gas form
  const gasForm = useForm<GasFormData>({
    defaultValues: {
      farmId: selectedFarmId || "",
      contractorId: "",
      invoiceNumber: invoiceData.invoiceNumber,
      invoiceDate: invoiceData.invoiceDate,
      invoiceTotal: invoiceData.grossAmount,
      unitPrice: 0,
      quantity: 0,
      comment: "",
    },
  });

  // Expense form
  const expenseForm = useForm<ExpenseFormData>({
    defaultValues: {
      farmId: selectedFarmId || "",
      cycleId: selectedCycleId || "",
      expenseContractorId: "",
      expenseTypeId: "",
      invoiceNumber: invoiceData.invoiceNumber,
      invoiceTotal: invoiceData.grossAmount,
      subTotal: invoiceData.netAmount,
      vatAmount: invoiceData.vatAmount,
      invoiceDate: invoiceData.invoiceDate,
      comment: "",
    },
  });

  // Sale form
  const saleForm = useForm<SaleFormData>({
    defaultValues: {
      farmId: selectedFarmId || "",
      cycleId: selectedCycleId || "",
      slaughterhouseId: "",
      invoiceNumber: invoiceData.invoiceNumber,
      invoiceDate: invoiceData.invoiceDate,
      dueDate: invoiceData.dueDate || "",
      invoiceTotal: invoiceData.grossAmount,
      subTotal: invoiceData.netAmount,
      vatAmount: invoiceData.vatAmount,
    },
  });

  const watchedFeedFarmId = feedForm.watch("farmId");
  const watchedGasFarmId = gasForm.watch("farmId");
  const watchedExpenseFarmId = expenseForm.watch("farmId");
  const watchedSaleFarmId = saleForm.watch("farmId");

  // Load cycles when farm changes
  useEffect(() => {
    const farmId =
      watchedFeedFarmId ||
      watchedGasFarmId ||
      watchedExpenseFarmId ||
      watchedSaleFarmId;
    if (farmId) {
      handleApiResponse(
        () => FarmsService.getFarmCycles(farmId),
        (data) => setCycles(data.responseData ?? []),
        () => setCycles([])
      );
    }
  }, [
    watchedFeedFarmId,
    watchedGasFarmId,
    watchedExpenseFarmId,
    watchedSaleFarmId,
  ]);

  // Load henhouses for feed module
  useEffect(() => {
    if (moduleType === ModuleType.Feeds && watchedFeedFarmId) {
      const farm = farms.find((f) => f.id === watchedFeedFarmId);
      if (farm?.henhouses) {
        setHenhouses(farm.henhouses);
      }
    }
  }, [moduleType, watchedFeedFarmId, farms]);

  // Load feed names
  useEffect(() => {
    if (moduleType === ModuleType.Feeds) {
      handleApiResponse(
        () => FeedsService.getFeedsNames(),
        (data) => setFeedNames(data.responseData?.fields ?? []),
        () => setFeedNames([])
      );
    }
  }, [moduleType]);

  // Load gas contractors
  useEffect(() => {
    if (moduleType === ModuleType.Gas) {
      handleApiResponse(
        () => GasService.getGasContractors(),
        (data) => {
          const contractors = data.responseData?.contractors ?? [];
          setGasContractors(contractors);
          // Auto-select contractor by NIP
          const contractor = contractors.find(
            (c) =>
              c.nip?.replace(/\D/g, "") ===
              invoiceData.sellerNip?.replace(/\D/g, "")
          );
          if (contractor) {
            gasForm.setValue("contractorId", contractor.id);
          }
        },
        () => setGasContractors([])
      );
    }
  }, [moduleType, invoiceData.sellerNip, gasForm]);

  // Load expense contractors
  useEffect(() => {
    if (moduleType === ModuleType.ProductionExpenses) {
      handleApiResponse(
        () => ExpensesService.getExpensesContractors({}),
        (data) => {
          const contractors = data.responseData?.contractors ?? [];
          setExpenseContractors(contractors);
          // Auto-select contractor by NIP
          const contractor = contractors.find(
            (c) =>
              c.nip?.replace(/\D/g, "") ===
              invoiceData.sellerNip?.replace(/\D/g, "")
          );
          if (contractor) {
            expenseForm.setValue("expenseContractorId", contractor.id);
          }
        },
        () => setExpenseContractors([])
      );
    }
  }, [moduleType, invoiceData.sellerNip, expenseForm]);

  // Load slaughterhouses
  useEffect(() => {
    if (moduleType === ModuleType.Sales) {
      handleApiResponse(
        () => SlaughterhousesService.getAllSlaughterhouses(),
        (data) => {
          const items = data.responseData?.items ?? [];
          setSlaughterhouses(items);
          // Auto-select slaughterhouse by NIP (buyer for sales)
          const slaughterhouse = items.find(
            (s) =>
              s.nip?.replace(/\D/g, "") ===
              invoiceData.buyerNip?.replace(/\D/g, "")
          );
          if (slaughterhouse) {
            saleForm.setValue("slaughterhouseId", slaughterhouse.id);
          }
        },
        () => setSlaughterhouses([])
      );
    }
  }, [moduleType, invoiceData.buyerNip, saleForm]);

  // Load expense types
  useEffect(() => {
    if (moduleType === ModuleType.ProductionExpenses) {
      handleApiResponse(
        () => ExpensesService.getExpensesTypes(),
        (data) => {
          setExpenseTypes(data.responseData?.types ?? []);
        },
        () => setExpenseTypes([])
      );
    }
  }, [moduleType]);

  // Calculate quantity and unit price from invoice line items
  useEffect(() => {
    if (!invoiceData.lineItems || invoiceData.lineItems.length === 0) return;

    const totalQuantity = invoiceData.lineItems.reduce(
      (sum, item) => sum + (item.quantity || 0),
      0
    );

    switch (moduleType) {
      case ModuleType.Feeds:
        // For feeds: sum quantity, unit price = 1
        feedForm.setValue("quantity", totalQuantity);
        feedForm.setValue("unitPrice", 1);
        break;
      case ModuleType.Gas:
        // For gas: sum quantity, unit price = grossAmount / quantity
        gasForm.setValue("quantity", totalQuantity);
        if (totalQuantity > 0) {
          gasForm.setValue(
            "unitPrice",
            invoiceData.grossAmount / totalQuantity
          );
        }
        break;
      case ModuleType.ProductionExpenses:
        // For expenses: no quantity/unitPrice fields, only set invoiceTotal
        expenseForm.setValue("invoiceTotal", invoiceData.grossAmount);
        break;
      case ModuleType.Sales:
        // For sales: no quantity/unitPrice fields, only set invoiceTotal
        saleForm.setValue("invoiceTotal", invoiceData.grossAmount);
        break;
    }
  }, [
    moduleType,
    invoiceData.lineItems,
    invoiceData.grossAmount,
    feedForm,
    gasForm,
    expenseForm,
    saleForm,
  ]);

  const handleSubmit = async () => {
    setLoading(true);
    try {
      // ...
      const request: CreateModuleEntityRequest | AcceptInvoiceRequest = {
        moduleType,
      };

      switch (moduleType) {
        case ModuleType.Feeds: {
          const data = feedForm.getValues();
          request.feedData = {
            farmId: data.farmId,
            cycleId: data.cycleId,
            henhouseId: data.henhouseId,
            invoiceNumber: data.invoiceNumber,
            bankAccountNumber: data.bankAccountNumber,
            vendorName: data.vendorName,
            itemName: data.itemName,
            quantity: data.quantity,
            unitPrice: data.unitPrice,
            invoiceDate: data.invoiceDate,
            dueDate: data.dueDate,
            invoiceTotal: data.invoiceTotal,
            subTotal: data.subTotal,
            vatAmount: data.vatAmount,
            comment: data.comment,
          };
          break;
        }
        case ModuleType.Gas: {
          const data = gasForm.getValues();
          request.gasData = {
            farmId: data.farmId,
            contractorId: data.contractorId || undefined,
            contractorNip: invoiceData.sellerNip,
            contractorName: invoiceData.sellerName,
            invoiceNumber: data.invoiceNumber,
            invoiceDate: data.invoiceDate,
            invoiceTotal: data.invoiceTotal,
            unitPrice: data.unitPrice,
            quantity: data.quantity,
            comment: data.comment,
          };
          break;
        }
        case ModuleType.ProductionExpenses: {
          const data = expenseForm.getValues();
          request.expenseData = {
            farmId: data.farmId,
            cycleId: data.cycleId,
            expenseContractorId: data.expenseContractorId || undefined,
            expenseTypeId: data.expenseTypeId,
            contractorNip: invoiceData.sellerNip,
            contractorName: invoiceData.sellerName,
            invoiceNumber: data.invoiceNumber,
            invoiceTotal: data.invoiceTotal,
            subTotal: data.subTotal,
            vatAmount: data.vatAmount,
            invoiceDate: data.invoiceDate,
            comment: data.comment,
          };
          break;
        }
        case ModuleType.Sales: {
          const data = saleForm.getValues();
          request.saleData = {
            farmId: data.farmId,
            cycleId: data.cycleId,
            slaughterhouseId: data.slaughterhouseId || undefined,
            slaughterhouseNip: invoiceData.buyerNip,
            slaughterhouseName: invoiceData.buyerName,
            invoiceNumber: data.invoiceNumber,
            invoiceDate: data.invoiceDate,
            dueDate: data.dueDate,
            invoiceTotal: data.invoiceTotal,
            subTotal: data.subTotal,
            vatAmount: data.vatAmount,
          };
          break;
        }
      }

      if (mode === "accept") {
        // Accept mode: changes status to Accepted AND creates module entity
        await handleApiResponse(
          () =>
            AccountingService.acceptInvoice(
              invoiceId,
              request as AcceptInvoiceRequest
            ),
          () => {
            toast.success(
              "Faktura została zaakceptowana i utworzono wpis w module"
            );
            onSuccess();
          },
          undefined,
          "Wystąpił błąd podczas akceptacji faktury"
        );
      } else {
        // Create mode: only creates module entity without changing status
        await handleApiResponse(
          () => AccountingService.createModuleEntity(invoiceId, request),
          () => {
            toast.success("Pomyślnie utworzono wpis w module");
            onSuccess();
          },
          undefined,
          "Wystąpił błąd podczas tworzenia wpisu w module"
        );
      }
    } finally {
      setLoading(false);
    }
  };

  const renderFeedForm = () => (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      <Typography variant="subtitle2" fontWeight={600}>
        Formularz dostawy paszy
      </Typography>

      <FormControl fullWidth size="small" required>
        <InputLabel>Ferma</InputLabel>
        <Select
          value={feedForm.watch("farmId")}
          label="Ferma"
          onChange={(e) => feedForm.setValue("farmId", e.target.value)}
        >
          {farms.map((farm) => (
            <MenuItem key={farm.id} value={farm.id}>
              {farm.name}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <FormControl
        fullWidth
        size="small"
        required
        disabled={!watchedFeedFarmId}
      >
        <InputLabel>Cykl</InputLabel>
        <Select
          value={feedForm.watch("cycleId")}
          label="Cykl"
          onChange={(e) => feedForm.setValue("cycleId", e.target.value)}
        >
          {cycles.map((cycle) => (
            <MenuItem key={cycle.id} value={cycle.id}>
              {cycle.identifier}/{cycle.year}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <FormControl
        fullWidth
        size="small"
        required
        disabled={!watchedFeedFarmId}
      >
        <InputLabel>Kurnik</InputLabel>
        <Select
          value={feedForm.watch("henhouseId")}
          label="Kurnik"
          onChange={(e) => feedForm.setValue("henhouseId", e.target.value)}
        >
          {henhouses.map((h) => (
            <MenuItem key={h.id} value={h.id}>
              {h.name}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <Autocomplete
        freeSolo
        options={feedNames.map((f) => f.name)}
        value={feedForm.watch("itemName")}
        onChange={(_, value) => feedForm.setValue("itemName", value || "")}
        onInputChange={(_, value) => feedForm.setValue("itemName", value)}
        renderInput={(params) => (
          <TextField {...params} label="Nazwa paszy" size="small" required />
        )}
      />

      <TextField
        label="Numer faktury"
        size="small"
        value={feedForm.watch("invoiceNumber")}
        onChange={(e) => feedForm.setValue("invoiceNumber", e.target.value)}
        required
      />

      <TextField
        label="Dostawca"
        size="small"
        value={feedForm.watch("vendorName")}
        onChange={(e) => feedForm.setValue("vendorName", e.target.value)}
        required
      />

      <Box sx={{ display: "flex", gap: 2 }}>
        <TextField
          label="Ilość [kg]"
          size="small"
          type="number"
          value={feedForm.watch("quantity")}
          onChange={(e) =>
            feedForm.setValue("quantity", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
          required
        />
        <TextField
          label="Cena jedn. [zł/kg]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={feedForm.watch("unitPrice")}
          onChange={(e) =>
            feedForm.setValue("unitPrice", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
          required
        />
      </Box>

      <Box sx={{ display: "flex", gap: 2 }}>
        <TextField
          label="Netto [zł]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={feedForm.watch("subTotal")}
          onChange={(e) =>
            feedForm.setValue("subTotal", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
        />
        <TextField
          label="VAT [zł]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={feedForm.watch("vatAmount")}
          onChange={(e) =>
            feedForm.setValue("vatAmount", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
        />
        <TextField
          label="Brutto [zł]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={feedForm.watch("invoiceTotal")}
          onChange={(e) =>
            feedForm.setValue("invoiceTotal", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
        />
      </Box>

      <TextField
        label="Nr konta bankowego"
        size="small"
        value={feedForm.watch("bankAccountNumber")}
        onChange={(e) => feedForm.setValue("bankAccountNumber", e.target.value)}
      />

      <TextField
        label="Komentarz"
        size="small"
        multiline
        rows={2}
        value={feedForm.watch("comment")}
        onChange={(e) => feedForm.setValue("comment", e.target.value)}
      />
    </Box>
  );

  const renderGasForm = () => (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      <Typography variant="subtitle2" fontWeight={600}>
        Formularz dostawy gazu
      </Typography>

      <FormControl fullWidth size="small" required>
        <InputLabel>Ferma</InputLabel>
        <Select
          value={gasForm.watch("farmId")}
          label="Ferma"
          onChange={(e) => gasForm.setValue("farmId", e.target.value)}
        >
          {farms.map((farm) => (
            <MenuItem key={farm.id} value={farm.id}>
              {farm.name}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <Autocomplete
        options={gasContractors}
        getOptionLabel={(option) => option.name || ""}
        value={
          gasContractors.find((c) => c.id === gasForm.watch("contractorId")) ||
          null
        }
        onChange={(_, value) =>
          gasForm.setValue("contractorId", value?.id || "")
        }
        renderInput={(params) => (
          <TextField {...params} label="Dostawca gazu" size="small" />
        )}
      />

      <TextField
        label="Numer faktury"
        size="small"
        value={gasForm.watch("invoiceNumber")}
        onChange={(e) => gasForm.setValue("invoiceNumber", e.target.value)}
        required
      />

      <Box sx={{ display: "flex", gap: 2 }}>
        <TextField
          label="Ilość [m³]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={gasForm.watch("quantity")}
          onChange={(e) =>
            gasForm.setValue("quantity", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
          required
        />
        <TextField
          label="Cena jedn. [zł/m³]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={gasForm.watch("unitPrice")}
          onChange={(e) =>
            gasForm.setValue("unitPrice", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
          required
        />
      </Box>

      <TextField
        label="Kwota brutto [zł]"
        size="small"
        type="number"
        inputProps={{ step: "0.01" }}
        value={gasForm.watch("invoiceTotal")}
        onChange={(e) =>
          gasForm.setValue("invoiceTotal", parseFloat(e.target.value) || 0)
        }
      />

      <TextField
        label="Komentarz"
        size="small"
        multiline
        rows={2}
        value={gasForm.watch("comment")}
        onChange={(e) => gasForm.setValue("comment", e.target.value)}
      />
    </Box>
  );

  const renderExpenseForm = () => (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      <Typography variant="subtitle2" fontWeight={600}>
        Formularz kosztu produkcyjnego
      </Typography>

      <FormControl fullWidth size="small" required>
        <InputLabel>Ferma</InputLabel>
        <Select
          value={expenseForm.watch("farmId")}
          label="Ferma"
          onChange={(e) => expenseForm.setValue("farmId", e.target.value)}
        >
          {farms.map((farm) => (
            <MenuItem key={farm.id} value={farm.id}>
              {farm.name}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <FormControl
        fullWidth
        size="small"
        required
        disabled={!watchedExpenseFarmId}
      >
        <InputLabel>Cykl</InputLabel>
        <Select
          value={expenseForm.watch("cycleId")}
          label="Cykl"
          onChange={(e) => expenseForm.setValue("cycleId", e.target.value)}
        >
          {cycles.map((cycle) => (
            <MenuItem key={cycle.id} value={cycle.id}>
              {cycle.identifier}/{cycle.year}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <Autocomplete
        options={expenseContractors}
        getOptionLabel={(option) => option.name || ""}
        value={
          expenseContractors.find(
            (c) => c.id === expenseForm.watch("expenseContractorId")
          ) || null
        }
        onChange={(_, value) => {
          expenseForm.setValue("expenseContractorId", value?.id || "");
          expenseForm.setValue("expenseTypeId", "");
        }}
        renderInput={(params) => (
          <TextField {...params} label="Kontrahent" size="small" />
        )}
      />

      <FormControl fullWidth size="small" required>
        <InputLabel>Typ wydatku</InputLabel>
        <Select
          value={expenseForm.watch("expenseTypeId")}
          label="Typ wydatku"
          onChange={(e) =>
            expenseForm.setValue("expenseTypeId", e.target.value)
          }
        >
          {(() => {
            const selectedContractor = expenseContractors.find(
              (c) => c.id === expenseForm.watch("expenseContractorId")
            );
            const availableTypes = selectedContractor?.expenseTypes || [];
            if (availableTypes.length > 0) {
              return availableTypes.map((type) => (
                <MenuItem key={type.id} value={type.id}>
                  {type.name}
                </MenuItem>
              ));
            }
            return expenseTypes.map((type) => (
              <MenuItem key={type.id} value={type.id}>
                {type.name}
              </MenuItem>
            ));
          })()}
        </Select>
      </FormControl>

      <TextField
        label="Numer faktury"
        size="small"
        value={expenseForm.watch("invoiceNumber")}
        onChange={(e) => expenseForm.setValue("invoiceNumber", e.target.value)}
        required
      />

      <Box sx={{ display: "flex", gap: 2 }}>
        <TextField
          label="Netto [zł]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={expenseForm.watch("subTotal")}
          onChange={(e) =>
            expenseForm.setValue("subTotal", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
        />
        <TextField
          label="VAT [zł]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={expenseForm.watch("vatAmount")}
          onChange={(e) =>
            expenseForm.setValue("vatAmount", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
        />
        <TextField
          label="Brutto [zł]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={expenseForm.watch("invoiceTotal")}
          onChange={(e) =>
            expenseForm.setValue(
              "invoiceTotal",
              parseFloat(e.target.value) || 0
            )
          }
          sx={{ flex: 1 }}
        />
      </Box>

      <TextField
        label="Komentarz"
        size="small"
        multiline
        rows={2}
        value={expenseForm.watch("comment")}
        onChange={(e) => expenseForm.setValue("comment", e.target.value)}
      />
    </Box>
  );

  const renderSaleForm = () => (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      <Typography variant="subtitle2" fontWeight={600}>
        Formularz faktury sprzedaży
      </Typography>

      <FormControl fullWidth size="small" required>
        <InputLabel>Ferma</InputLabel>
        <Select
          value={saleForm.watch("farmId")}
          label="Ferma"
          onChange={(e) => saleForm.setValue("farmId", e.target.value)}
        >
          {farms.map((farm) => (
            <MenuItem key={farm.id} value={farm.id}>
              {farm.name}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <FormControl
        fullWidth
        size="small"
        required
        disabled={!watchedSaleFarmId}
      >
        <InputLabel>Cykl</InputLabel>
        <Select
          value={saleForm.watch("cycleId")}
          label="Cykl"
          onChange={(e) => saleForm.setValue("cycleId", e.target.value)}
        >
          {cycles.map((cycle) => (
            <MenuItem key={cycle.id} value={cycle.id}>
              {cycle.identifier}/{cycle.year}
            </MenuItem>
          ))}
        </Select>
      </FormControl>

      <Autocomplete
        options={slaughterhouses}
        getOptionLabel={(option) => option.name || ""}
        value={
          slaughterhouses.find(
            (s) => s.id === saleForm.watch("slaughterhouseId")
          ) || null
        }
        onChange={(_, value) =>
          saleForm.setValue("slaughterhouseId", value?.id || "")
        }
        renderInput={(params) => (
          <TextField {...params} label="Ubojnia" size="small" required />
        )}
      />

      <TextField
        label="Numer faktury"
        size="small"
        value={saleForm.watch("invoiceNumber")}
        onChange={(e) => saleForm.setValue("invoiceNumber", e.target.value)}
        required
      />

      <Box sx={{ display: "flex", gap: 2 }}>
        <TextField
          label="Netto [zł]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={saleForm.watch("subTotal")}
          onChange={(e) =>
            saleForm.setValue("subTotal", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
        />
        <TextField
          label="VAT [zł]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={saleForm.watch("vatAmount")}
          onChange={(e) =>
            saleForm.setValue("vatAmount", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
        />
        <TextField
          label="Brutto [zł]"
          size="small"
          type="number"
          inputProps={{ step: "0.01" }}
          value={saleForm.watch("invoiceTotal")}
          onChange={(e) =>
            saleForm.setValue("invoiceTotal", parseFloat(e.target.value) || 0)
          }
          sx={{ flex: 1 }}
        />
      </Box>
    </Box>
  );

  const renderForm = () => {
    switch (moduleType) {
      case ModuleType.Feeds:
        return renderFeedForm();
      case ModuleType.Gas:
        return renderGasForm();
      case ModuleType.ProductionExpenses:
        return renderExpenseForm();
      case ModuleType.Sales:
        return renderSaleForm();
      default:
        return null;
    }
  };

  // Don't show form for Farmstead, Other, or None
  if (
    moduleType === ModuleType.Farmstead ||
    moduleType === ModuleType.Other ||
    moduleType === ModuleType.None
  ) {
    return null;
  }

  return (
    <Box sx={{ mt: 2, p: 2, bgcolor: "action.hover", borderRadius: 1 }}>
      {renderForm()}
      <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
        <Button
          variant="contained"
          color="primary"
          onClick={handleSubmit}
          disabled={loading}
          size="small"
        >
          {loading ? <CircularProgress size={20} /> : "Utwórz wpis"}
        </Button>
        <Button
          variant="outlined"
          onClick={onCancel}
          size="small"
          disabled={loading}
        >
          Anuluj
        </Button>
      </Box>
    </Box>
  );
};

export default ModuleEntityForm;
