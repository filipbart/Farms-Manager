export type ModuleType =
  | "None"
  | "Feeds"
  | "ProductionExpenses"
  | "Gas"
  | "Sales"
  | "Farmstead"
  | "Investments"
  | "RealEstate"
  | "Other";

export type KSeFInvoiceDirection = "Purchase" | "Sales";

export interface InvoiceModuleAssignmentRule {
  id: string;
  name: string;
  description?: string;
  priority: number;
  targetModule: ModuleType;
  targetModuleName: string;
  includeKeywords: string[];
  excludeKeywords: string[];
  taxBusinessEntityId?: string;
  taxBusinessEntityName?: string;
  farmId?: string;
  farmName?: string;
  invoiceDirection?: KSeFInvoiceDirection;
  invoiceDirectionName?: string;
  isActive: boolean;
  dateCreatedUtc: string;
}

export interface CreateInvoiceModuleAssignmentRuleDto {
  name: string;
  description?: string;
  targetModule: ModuleType;
  includeKeywords: string[];
  excludeKeywords: string[];
  taxBusinessEntityId?: string;
  farmId?: string;
  invoiceDirection?: KSeFInvoiceDirection;
}

export interface UpdateInvoiceModuleAssignmentRuleDto {
  name?: string;
  description?: string;
  targetModule?: ModuleType;
  includeKeywords?: string[];
  excludeKeywords?: string[];
  taxBusinessEntityId?: string;
  farmId?: string;
  invoiceDirection?: KSeFInvoiceDirection;
  isActive?: boolean;
  clearTaxBusinessEntity?: boolean;
  clearFarm?: boolean;
  clearInvoiceDirection?: boolean;
}

export const MODULE_TYPE_OPTIONS: { value: ModuleType; label: string }[] = [
  { value: "None", label: "Brak przypisanego" },
  { value: "Feeds", label: "Pasze" },
  { value: "ProductionExpenses", label: "Koszty produkcyjne" },
  { value: "Gas", label: "Gaz" },
  { value: "Sales", label: "Sprzedaże" },
  { value: "Farmstead", label: "Gospodarstwo rolne" },
  { value: "Investments", label: "Inwestycje" },
  { value: "RealEstate", label: "Nieruchomości" },
  { value: "Other", label: "Inne" },
];

export const INVOICE_DIRECTION_OPTIONS: {
  value: KSeFInvoiceDirection;
  label: string;
}[] = [
  { value: "Purchase", label: "Zakup" },
  { value: "Sales", label: "Sprzedaż" },
];
