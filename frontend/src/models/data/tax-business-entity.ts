import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface TaxBusinessEntityRowModel extends AuditFields {
  id: string;
  nip: string;
  name: string;
  businessType: string;
  description: string | null;
  hasKSeFToken: boolean;
}

export interface AddTaxBusinessEntityFormData {
  nip: string;
  name: string;
  businessType: string;
  description: string;
  kSeFToken: string;
}

export interface UpdateTaxBusinessEntityFormData {
  name: string;
  businessType: string;
  description: string;
  kSeFToken: string;
}
