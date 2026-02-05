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
  formatNumberWithSpaces,
  parseFormattedNumber,
} from "../../../utils/number-format";
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
    additionalDescriptions?: { key?: string; value?: string }[];
    bankAccountNumber?: string;
    gasQuantity?: number | null;
    gasUnitPrice?: number | null;
    feedHenhouseId?: string | null;
  };
  farms: FarmRowModel[];
  selectedFarmId: string;
  selectedCycleId: string;
  comment?: string;
  onSuccess: () => void;
  onDataChange?: (data: {
    henhouseId?: string;
    invoiceNumber?: string;
  }) => void;
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
      onDataChange,
      mode = "create",
    },
    ref,
  ) => {
    const [loading, setLoading] = useState(false);
    const [henhouses, setHenhouses] = useState<{ id: string; name: string }[]>(
      [],
    );
    const [feedNames, setFeedNames] = useState<FeedsNamesRow[]>([]);
    const [gasContractors, setGasContractors] = useState<GasContractorRow[]>(
      [],
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
        bankAccountNumber: invoiceData.bankAccountNumber || "",
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
        unitPrice: invoiceData.gasUnitPrice || 0,
        quantity: invoiceData.gasQuantity || 0,
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

    // Watch for form changes and notify parent
    const watchedFeedHenhouseId = feedForm.watch("henhouseId");
    const watchedFeedInvoiceNumber = feedForm.watch("invoiceNumber");

    useEffect(() => {
      if (moduleType === ModuleType.Feeds && onDataChange) {
        onDataChange({
          henhouseId: watchedFeedHenhouseId,
          invoiceNumber: watchedFeedInvoiceNumber,
        });
      }
    }, [
      moduleType,
      watchedFeedHenhouseId,
      watchedFeedInvoiceNumber,
      onDataChange,
    ]);

    const watchedGasInvoiceNumber = gasForm.watch("invoiceNumber");

    useEffect(() => {
      if (moduleType === ModuleType.Gas && onDataChange) {
        onDataChange({
          invoiceNumber: watchedGasInvoiceNumber,
        });
      }
    }, [moduleType, watchedGasInvoiceNumber, onDataChange]);

    const watchedExpenseInvoiceNumber = expenseForm.watch("invoiceNumber");

    useEffect(() => {
      if (moduleType === ModuleType.ProductionExpenses && onDataChange) {
        onDataChange({
          invoiceNumber: watchedExpenseInvoiceNumber,
        });
      }
    }, [moduleType, watchedExpenseInvoiceNumber, onDataChange]);

    const watchedSaleInvoiceNumber = saleForm.watch("invoiceNumber");

    useEffect(() => {
      if (moduleType === ModuleType.Sales && onDataChange) {
        onDataChange({
          invoiceNumber: watchedSaleInvoiceNumber,
        });
      }
    }, [moduleType, watchedSaleInvoiceNumber, onDataChange]);

    // Load henhouses for feed module and auto-select from database ID
    useEffect(() => {
      if (moduleType === ModuleType.Feeds && selectedFarmId) {
        const farm = farms.find((f) => f.id === selectedFarmId);
        if (farm?.henhouses) {
          setHenhouses(farm.henhouses);

          // Use henhouse ID from database if available
          if (invoiceData.feedHenhouseId && !feedForm.getValues("henhouseId")) {
            // Verify the henhouse exists in this farm
            const henhouseExists = farm.henhouses.some(
              (h) => h.id === invoiceData.feedHenhouseId,
            );
            if (henhouseExists) {
              feedForm.setValue("henhouseId", invoiceData.feedHenhouseId);
            }
          }
        }
      }
    }, [
      moduleType,
      selectedFarmId,
      farms,
      invoiceData.feedHenhouseId,
      feedForm,
    ]);

    // Load feed names
    useEffect(() => {
      if (moduleType === ModuleType.Feeds) {
        handleApiResponse(
          () => FeedsService.getFeedsNames(),
          (data) => setFeedNames(data.responseData?.fields ?? []),
          () => setFeedNames([]),
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
              "",
            );
            let contractor = contractors.find(
              (c) => c.nip?.replace(/\D/g, "") === normalizedInvoiceNip,
            );

            // Fallback: match by name (case-insensitive, partial match)
            if (!contractor && invoiceData.sellerName) {
              const sellerNameLower = invoiceData.sellerName.toLowerCase();
              contractor = contractors.find(
                (c) =>
                  c.name?.toLowerCase().includes(sellerNameLower) ||
                  sellerNameLower.includes(c.name?.toLowerCase() || ""),
              );
            }

            if (contractor) {
              gasForm.setValue("contractorId", contractor.id);
            }
          },
          () => setGasContractors([]),
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
              "",
            );
            let contractor = contractors.find(
              (c) => c.nip?.replace(/\D/g, "") === normalizedInvoiceNip,
            );

            // Fallback: match by name (case-insensitive, partial match)
            if (!contractor && invoiceData.sellerName) {
              const sellerNameLower = invoiceData.sellerName.toLowerCase();
              contractor = contractors.find(
                (c) =>
                  c.name?.toLowerCase().includes(sellerNameLower) ||
                  sellerNameLower.includes(c.name?.toLowerCase() || ""),
              );
            }

            if (contractor) {
              expenseForm.setValue("expenseContractorId", contractor.id);
            }
          },
          () => setExpenseContractors([]),
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
              "",
            );
            let slaughterhouse = items.find(
              (s) => s.nip?.replace(/\D/g, "") === normalizedInvoiceNip,
            );

            // Fallback: match by name (case-insensitive, partial match)
            if (!slaughterhouse && invoiceData.buyerName) {
              const buyerNameLower = invoiceData.buyerName.toLowerCase();
              slaughterhouse = items.find(
                (s) =>
                  s.name?.toLowerCase().includes(buyerNameLower) ||
                  buyerNameLower.includes(s.name?.toLowerCase() || ""),
              );
            }

            if (slaughterhouse) {
              saleForm.setValue("slaughterhouseId", slaughterhouse.id);
            }
          },
          () => setSlaughterhouses([]),
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
          () => setExpenseTypes([]),
        );
      }
    }, [moduleType]);

    // Auto-select expense type if only one available
    const watchedExpenseContractorId = expenseForm.watch("expenseContractorId");
    useEffect(() => {
      if (moduleType === ModuleType.ProductionExpenses) {
        const currentTypeId = expenseForm.getValues("expenseTypeId");
        let availableTypes: { id: string }[] = [];

        if (watchedExpenseContractorId) {
          const selectedContractor = expenseContractors.find(
            (c) => c.id === watchedExpenseContractorId,
          );
          if (
            selectedContractor?.expenseTypes &&
            selectedContractor.expenseTypes.length > 0
          ) {
            availableTypes = selectedContractor.expenseTypes;
          } else {
            availableTypes = expenseTypes;
          }
        } else {
          availableTypes = expenseTypes;
        }

        if (availableTypes.length === 1) {
          if (currentTypeId !== availableTypes[0].id) {
            expenseForm.setValue("expenseTypeId", availableTypes[0].id, {
              shouldValidate: true,
              shouldDirty: true,
            });
          }
        }
      }
    }, [
      moduleType,
      watchedExpenseContractorId,
      expenseContractors,
      expenseTypes,
      expenseForm,
    ]);

    // Calculate quantity and unit price from invoice line items
    useEffect(() => {
      if (!invoiceData.lineItems || invoiceData.lineItems.length === 0) return;

      const totalQuantity = invoiceData.lineItems.reduce(
        (sum, item) => sum + (item.quantity || 0),
        0,
      );

      switch (moduleType) {
        case ModuleType.Feeds: {
          // For feeds: sum quantity, get unit price from line items or calculate
          feedForm.setValue("quantity", totalQuantity);

          // Try to get unit price from first line item
          const firstItem = invoiceData.lineItems[0];
          if (firstItem?.unitPriceNet && firstItem.unitPriceNet > 0) {
            // Use unit price from line item (weighted average if multiple items)
            const totalNetAmount = invoiceData.lineItems.reduce(
              (sum, item) => sum + (item.netAmount || 0),
              0,
            );
            if (totalQuantity > 0) {
              const avgUnitPrice = totalNetAmount / totalQuantity;
              feedForm.setValue(
                "unitPrice",
                Math.round(avgUnitPrice * 100) / 100,
              );
            }
          } else if (totalQuantity > 0) {
            // Fallback: calculate from total net amount
            const avgUnitPrice = invoiceData.netAmount / totalQuantity;
            feedForm.setValue(
              "unitPrice",
              Math.round(avgUnitPrice * 100) / 100,
            );
          }

          // Auto-detect feed name from first line item
          if (firstItem?.name && !feedForm.getValues("itemName")) {
            feedForm.setValue("itemName", firstItem.name);
          }
          break;
        }
        case ModuleType.Gas: {
          // For gas: use pre-extracted data if available (Manual invoices), otherwise calculate from line items
          if (invoiceData.gasQuantity && invoiceData.gasUnitPrice) {
            // Use pre-extracted data from backend (for Manual invoices)
            gasForm.setValue("quantity", invoiceData.gasQuantity);
            gasForm.setValue("unitPrice", invoiceData.gasUnitPrice);
          } else {
            // Calculate from line items (for KSeF invoices)
            gasForm.setValue("quantity", totalQuantity);

            // Try to get unit price from first line item
            const firstGasItem = invoiceData.lineItems?.[0];
            if (
              firstGasItem?.unitPriceGross &&
              firstGasItem.unitPriceGross > 0
            ) {
              // Use unit price from line item
              gasForm.setValue(
                "unitPrice",
                Math.round(firstGasItem.unitPriceGross * 100) / 100,
              );
            } else if (
              firstGasItem?.unitPriceNet &&
              firstGasItem.unitPriceNet > 0
            ) {
              // Use net price if gross not available
              gasForm.setValue(
                "unitPrice",
                Math.round(firstGasItem.unitPriceNet * 100) / 100,
              );
            } else if (totalQuantity > 0) {
              // Fallback: calculate from gross amount
              gasForm.setValue(
                "unitPrice",
                Math.round((invoiceData.grossAmount / totalQuantity) * 100) /
                  100,
              );
            }
          }
          break;
        }
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
      invoiceData.netAmount,
      invoiceData.gasQuantity,
      invoiceData.gasUnitPrice,
      feedForm,
      gasForm,
      expenseForm,
      saleForm,
    ]);

    const validateForm = (): string | null => {
      switch (moduleType) {
        case ModuleType.Feeds: {
          const data = feedForm.getValues();
          if (!selectedFarmId) return "Wybierz fermę";
          if (!selectedCycleId) return "Wybierz cykl";
          if (!data.henhouseId) return "Wybierz kurnik";
          if (!data.itemName) return "Wybierz nazwę paszy";
          if (!data.quantity || data.quantity <= 0) return "Podaj ilość";
          if (!data.unitPrice || data.unitPrice <= 0)
            return "Podaj cenę jednostkową";
          break;
        }
        case ModuleType.Gas: {
          const data = gasForm.getValues();
          if (!selectedFarmId) return "Wybierz fermę";
          if (!data.quantity || data.quantity <= 0) return "Podaj ilość";
          if (!data.unitPrice || data.unitPrice <= 0)
            return "Podaj cenę jednostkową";
          break;
        }
        case ModuleType.ProductionExpenses: {
          const data = expenseForm.getValues();
          if (!selectedFarmId) return "Wybierz fermę";
          if (!selectedCycleId) return "Wybierz cykl";
          if (!data.expenseTypeId) return "Wybierz typ kosztu";
          break;
        }
        case ModuleType.Sales: {
          if (!selectedFarmId) return "Wybierz fermę";
          if (!selectedCycleId) return "Wybierz cykl";
          break;
        }
      }
      return null;
    };

    const handleSubmit = async () => {
      const validationError = validateForm();
      if (validationError) {
        toast.error(validationError);
        return;
      }

      setLoading(true);
      try {
        const request: CreateModuleEntityRequest | AcceptInvoiceRequest = {
          moduleType,
        };

        switch (moduleType) {
          case ModuleType.Feeds: {
            const data = feedForm.getValues();
            request.feedData = {
              invoiceId,
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
              dueDate: data.dueDate || invoiceData.dueDate || "",
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
              invoiceId,
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
              invoiceId,
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
              invoiceId,
              farmId: selectedFarmId,
              cycleId: selectedCycleId,
              slaughterhouseId: data.slaughterhouseId || undefined,
              slaughterhouseNip: invoiceData.buyerNip,
              slaughterhouseName: invoiceData.buyerName,
              invoiceNumber: data.invoiceNumber,
              invoiceDate: data.invoiceDate,
              dueDate: data.dueDate || invoiceData.dueDate || "",
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
                request as AcceptInvoiceRequest,
              ),
            () => {
              toast.success(
                "Faktura została zaakceptowana i utworzono wpis w module",
              );
              onSuccess();
            },
            undefined,
            "Wystąpił błąd podczas akceptacji faktury",
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
            "Wystąpił błąd podczas tworzenia wpisu w module",
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
            value={formatNumberWithSpaces(feedForm.watch("quantity") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              feedForm.setValue("quantity", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
            required
          />
          <TextField
            label="Cena jedn. [zł/kg]"
            size="small"
            value={formatNumberWithSpaces(feedForm.watch("unitPrice") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              feedForm.setValue("unitPrice", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
            required
          />
        </Box>

        <Box sx={{ display: "flex", gap: 2 }}>
          <TextField
            label="Netto [zł]"
            size="small"
            value={formatNumberWithSpaces(feedForm.watch("subTotal") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              feedForm.setValue("subTotal", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
          />
          <TextField
            label="VAT [zł]"
            size="small"
            value={formatNumberWithSpaces(feedForm.watch("vatAmount") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              feedForm.setValue("vatAmount", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
          />
          <TextField
            label="Brutto [zł]"
            size="small"
            value={formatNumberWithSpaces(feedForm.watch("invoiceTotal") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              feedForm.setValue("invoiceTotal", parsed ? Number(parsed) : 0);
            }}
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
              (c) => c.id === gasForm.watch("contractorId"),
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
            label="Ilość [l]"
            size="small"
            value={formatNumberWithSpaces(gasForm.watch("quantity") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              gasForm.setValue("quantity", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
            required
          />
          <TextField
            label="Cena jedn. [zł/l]"
            size="small"
            value={formatNumberWithSpaces(gasForm.watch("unitPrice") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              gasForm.setValue("unitPrice", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
            required
          />
        </Box>

        <TextField
          label="Kwota brutto [zł]"
          size="small"
          value={formatNumberWithSpaces(gasForm.watch("invoiceTotal") || "")}
          onChange={(e) => {
            const parsed = parseFormattedNumber(e.target.value);
            gasForm.setValue("invoiceTotal", parsed ? Number(parsed) : 0);
          }}
          required
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
              (c) => c.id === expenseForm.watch("expenseContractorId"),
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
                (c) => c.id === expenseForm.watch("expenseContractorId"),
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
            value={formatNumberWithSpaces(expenseForm.watch("subTotal") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              expenseForm.setValue("subTotal", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
          />
          <TextField
            label="VAT [zł]"
            size="small"
            value={formatNumberWithSpaces(expenseForm.watch("vatAmount") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              expenseForm.setValue("vatAmount", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
          />
          <TextField
            label="Brutto [zł]"
            size="small"
            value={formatNumberWithSpaces(
              expenseForm.watch("invoiceTotal") || "",
            )}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              expenseForm.setValue("invoiceTotal", parsed ? Number(parsed) : 0);
            }}
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
              (s) => s.id === saleForm.watch("slaughterhouseId"),
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
            value={formatNumberWithSpaces(saleForm.watch("subTotal") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              saleForm.setValue("subTotal", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
          />
          <TextField
            label="VAT [zł]"
            size="small"
            value={formatNumberWithSpaces(saleForm.watch("vatAmount") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              saleForm.setValue("vatAmount", parsed ? Number(parsed) : 0);
            }}
            sx={{ flex: 1 }}
          />
          <TextField
            label="Brutto [zł]"
            size="small"
            value={formatNumberWithSpaces(saleForm.watch("invoiceTotal") || "")}
            onChange={(e) => {
              const parsed = parseFormattedNumber(e.target.value);
              saleForm.setValue("invoiceTotal", parsed ? Number(parsed) : 0);
            }}
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
      <Box
        sx={{ mt: 2, p: 2, borderRadius: 1, border: 1, borderColor: "divider" }}
      >
        {renderForm()}
      </Box>
    );
  },
);

export default ModuleEntityForm;
