import { useEffect, useState, forwardRef, useImperativeHandle } from "react";
import {
  Box,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Typography,
  Autocomplete,
} from "@mui/material";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import dayjs from "dayjs";
import {
  AccountingService,
  type CreateModuleEntityRequest,
  type AcceptInvoiceRequest,
} from "../../../services/accounting-service";
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
    footer?: string;
  };
  farms: FarmRowModel[];
  selectedFarmId: string;
  selectedCycleId: string;
  comment?: string;
  onSuccess: () => void;
  /**
   * Mode: 'create' - creates module entity without changing status
   * Mode: 'accept' - creates module entity AND changes status to Accepted
   */
  mode?: "create" | "accept";
}

export interface ModuleEntityFormRef {
  submit: () => Promise<void>;
  isLoading: () => boolean;
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

const ModuleEntityForm = forwardRef<ModuleEntityFormRef, ModuleEntityFormProps>(
  (
    {
      invoiceId,
      moduleType,
      invoiceData,
      farms,
      selectedFarmId,
      selectedCycleId,
      comment,
      onSuccess,
      mode = "create",
    },
    ref
  ) => {
    const [loading, setLoading] = useState(false);
    const [henhouses, setHenhouses] = useState<{ id: string; name: string }[]>(
      []
    );
    const [feedNames, setFeedNames] = useState<FeedsNamesRow[]>([]);
    const [gasContractors, setGasContractors] = useState<GasContractorRow[]>(
      []
    );
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
        invoiceDate: dayjs(invoiceData.invoiceDate).format("YYYY-MM-DD"),
        dueDate: invoiceData.dueDate
          ? dayjs(invoiceData.dueDate).format("YYYY-MM-DD")
          : "",
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
        invoiceDate: dayjs(invoiceData.invoiceDate).format("YYYY-MM-DD"),
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
        invoiceDate: dayjs(invoiceData.invoiceDate).format("YYYY-MM-DD"),
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
        invoiceDate: dayjs(invoiceData.invoiceDate).format("YYYY-MM-DD"),
        dueDate: invoiceData.dueDate
          ? dayjs(invoiceData.dueDate).format("YYYY-MM-DD")
          : "",
        invoiceTotal: invoiceData.grossAmount,
        subTotal: invoiceData.netAmount,
        vatAmount: invoiceData.vatAmount,
      },
    });

    // Load henhouses for feed module and auto-detect from invoice text
    useEffect(() => {
      if (moduleType === ModuleType.Feeds && selectedFarmId) {
        const farm = farms.find((f) => f.id === selectedFarmId);
        if (farm?.henhouses) {
          setHenhouses(farm.henhouses);

          // Auto-detect henhouse from invoice text (line items, seller name, footer/uwagi)
          const searchableText = [
            invoiceData.sellerName,
            invoiceData.buyerName,
            ...(invoiceData.lineItems?.map((item) => item.name) || []),
            invoiceData.footer, // Stopka faktury - może zawierać "Miejsce rozładunku: Jaworowo Kłódź K5"
          ]
            .filter(Boolean)
            .join(" ")
            .toLowerCase();

          // Find henhouse by name or code match in invoice text
          const matchedHenhouse = farm.henhouses.find((h) => {
            const henhouseName = h.name.toLowerCase();
            // Sprawdź czy nazwa kurnika występuje w tekście
            return searchableText.includes(henhouseName);
          });
          if (matchedHenhouse && !feedForm.getValues("henhouseId")) {
            feedForm.setValue("henhouseId", matchedHenhouse.id);
          }
        }
      }
    }, [moduleType, selectedFarmId, farms, invoiceData, feedForm]);

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

            // Auto-select contractor by NIP first
            const normalizedInvoiceNip = invoiceData.sellerNip?.replace(
              /\D/g,
              ""
            );
            let contractor = contractors.find(
              (c) => c.nip?.replace(/\D/g, "") === normalizedInvoiceNip
            );

            // Fallback: match by name (case-insensitive, partial match)
            if (!contractor && invoiceData.sellerName) {
              const sellerNameLower = invoiceData.sellerName.toLowerCase();
              contractor = contractors.find(
                (c) =>
                  c.name?.toLowerCase().includes(sellerNameLower) ||
                  sellerNameLower.includes(c.name?.toLowerCase() || "")
              );
            }

            if (contractor) {
              gasForm.setValue("contractorId", contractor.id);
            }
          },
          () => setGasContractors([])
        );
      }
    }, [moduleType, invoiceData.sellerNip, invoiceData.sellerName, gasForm]);

    // Load expense contractors
    useEffect(() => {
      if (moduleType === ModuleType.ProductionExpenses) {
        handleApiResponse(
          () => ExpensesService.getExpensesContractors({}),
          (data) => {
            const contractors = data.responseData?.contractors ?? [];
            setExpenseContractors(contractors);

            // Auto-select contractor by NIP first
            const normalizedInvoiceNip = invoiceData.sellerNip?.replace(
              /\D/g,
              ""
            );
            let contractor = contractors.find(
              (c) => c.nip?.replace(/\D/g, "") === normalizedInvoiceNip
            );

            // Fallback: match by name (case-insensitive, partial match)
            if (!contractor && invoiceData.sellerName) {
              const sellerNameLower = invoiceData.sellerName.toLowerCase();
              contractor = contractors.find(
                (c) =>
                  c.name?.toLowerCase().includes(sellerNameLower) ||
                  sellerNameLower.includes(c.name?.toLowerCase() || "")
              );
            }

            if (contractor) {
              expenseForm.setValue("expenseContractorId", contractor.id);
            }
          },
          () => setExpenseContractors([])
        );
      }
    }, [
      moduleType,
      invoiceData.sellerNip,
      invoiceData.sellerName,
      expenseForm,
    ]);

    // Load slaughterhouses
    useEffect(() => {
      if (moduleType === ModuleType.Sales) {
        handleApiResponse(
          () => SlaughterhousesService.getAllSlaughterhouses(),
          (data) => {
            const items = data.responseData?.items ?? [];
            setSlaughterhouses(items);

            // Auto-select slaughterhouse by NIP first (buyer for sales)
            const normalizedInvoiceNip = invoiceData.buyerNip?.replace(
              /\D/g,
              ""
            );
            let slaughterhouse = items.find(
              (s) => s.nip?.replace(/\D/g, "") === normalizedInvoiceNip
            );

            // Fallback: match by name (case-insensitive, partial match)
            if (!slaughterhouse && invoiceData.buyerName) {
              const buyerNameLower = invoiceData.buyerName.toLowerCase();
              slaughterhouse = items.find(
                (s) =>
                  s.name?.toLowerCase().includes(buyerNameLower) ||
                  buyerNameLower.includes(s.name?.toLowerCase() || "")
              );
            }

            if (slaughterhouse) {
              saleForm.setValue("slaughterhouseId", slaughterhouse.id);
            }
          },
          () => setSlaughterhouses([])
        );
      }
    }, [moduleType, invoiceData.buyerNip, invoiceData.buyerName, saleForm]);

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
        case ModuleType.Feeds: {
          // For feeds: sum quantity, calculate weighted average unit price
          feedForm.setValue("quantity", totalQuantity);

          // Calculate weighted average unit price from line items
          const totalNetAmount = invoiceData.lineItems.reduce(
            (sum, item) => sum + (item.netAmount || 0),
            0
          );
          if (totalQuantity > 0) {
            const avgUnitPrice = totalNetAmount / totalQuantity;
            feedForm.setValue(
              "unitPrice",
              Math.round(avgUnitPrice * 100) / 100
            );
          }

          // Auto-detect feed name from first line item
          const firstItem = invoiceData.lineItems[0];
          if (firstItem?.name && !feedForm.getValues("itemName")) {
            feedForm.setValue("itemName", firstItem.name);
          }
          break;
        }
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
              farmId: selectedFarmId,
              cycleId: selectedCycleId,
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
              comment: comment || "",
            };
            break;
          }
          case ModuleType.Gas: {
            const data = gasForm.getValues();
            request.gasData = {
              farmId: selectedFarmId,
              contractorId: data.contractorId || undefined,
              contractorNip: invoiceData.sellerNip,
              contractorName: invoiceData.sellerName,
              invoiceNumber: data.invoiceNumber,
              invoiceDate: data.invoiceDate,
              invoiceTotal: data.invoiceTotal,
              unitPrice: data.unitPrice,
              quantity: data.quantity,
              comment: comment || "",
            };
            break;
          }
          case ModuleType.ProductionExpenses: {
            const data = expenseForm.getValues();
            request.expenseData = {
              farmId: selectedFarmId,
              cycleId: selectedCycleId,
              expenseContractorId: data.expenseContractorId || undefined,
              expenseTypeId: data.expenseTypeId,
              contractorNip: invoiceData.sellerNip,
              contractorName: invoiceData.sellerName,
              invoiceNumber: data.invoiceNumber,
              invoiceTotal: data.invoiceTotal,
              subTotal: data.subTotal,
              vatAmount: data.vatAmount,
              invoiceDate: data.invoiceDate,
              comment: comment || "",
            };
            break;
          }
          case ModuleType.Sales: {
            const data = saleForm.getValues();
            request.saleData = {
              farmId: selectedFarmId,
              cycleId: selectedCycleId,
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

    // Expose submit method to parent via ref
    useImperativeHandle(ref, () => ({
      submit: handleSubmit,
      isLoading: () => loading,
    }));

    const renderFeedForm = () => (
      <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
        <Typography variant="subtitle2" fontWeight={600}>
          Formularz dostawy paszy
        </Typography>

        <FormControl fullWidth size="small" required>
          {" "}
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
          onChange={(e) =>
            feedForm.setValue("bankAccountNumber", e.target.value)
          }
        />
      </Box>
    );

    const renderGasForm = () => (
      <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
        <Typography variant="subtitle2" fontWeight={600}>
          Formularz dostawy gazu
        </Typography>

        <Autocomplete
          options={gasContractors}
          getOptionLabel={(option) => option.name || ""}
          value={
            gasContractors.find(
              (c) => c.id === gasForm.watch("contractorId")
            ) || null
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
      </Box>
    );

    const renderExpenseForm = () => (
      <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
        <Typography variant="subtitle2" fontWeight={600}>
          Formularz kosztu produkcyjnego
        </Typography>

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
          onChange={(e) =>
            expenseForm.setValue("invoiceNumber", e.target.value)
          }
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
      </Box>
    );

    const renderSaleForm = () => (
      <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
        <Typography variant="subtitle2" fontWeight={600}>
          Formularz faktury sprzedaży
        </Typography>

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
      </Box>
    );
  }
);

export default ModuleEntityForm;
