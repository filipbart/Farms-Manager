export type KSeFInvoiceDirection = "Purchase" | "Sale";

export interface InvoiceFarmAssignmentRule {
  id: string;
  name: string;
  description?: string;
  priority: number;
  targetFarmId: string;
  targetFarmName?: string;
  includeKeywords: string[];
  excludeKeywords: string[];
  taxBusinessEntityId?: string;
  taxBusinessEntityName?: string;
  invoiceDirection?: KSeFInvoiceDirection;
  invoiceDirectionName?: string;
  isActive: boolean;
  dateCreatedUtc: string;
}

export interface CreateInvoiceFarmAssignmentRuleDto {
  name: string;
  description?: string;
  targetFarmId: string;
  includeKeywords: string[];
  excludeKeywords: string[];
  taxBusinessEntityId?: string;
  invoiceDirection?: KSeFInvoiceDirection;
}

export interface UpdateInvoiceFarmAssignmentRuleDto {
  name?: string;
  description?: string;
  targetFarmId?: string;
  includeKeywords?: string[];
  excludeKeywords?: string[];
  taxBusinessEntityId?: string;
  invoiceDirection?: KSeFInvoiceDirection;
  isActive?: boolean;
  clearTaxBusinessEntity?: boolean;
  clearInvoiceDirection?: boolean;
}

export const INVOICE_DIRECTION_OPTIONS: {
  value: KSeFInvoiceDirection;
  label: string;
}[] = [
  { value: "Purchase", label: "Zakup" },
  { value: "Sale", label: "Sprzedaż" },
];
