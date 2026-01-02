import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface TaxBusinessEntityRowModel extends AuditFields {
  id: string;
  nip: string;
  name: string;
  businessType: string;
  description: string | null;
}

export interface AddTaxBusinessEntityFormData {
  nip: string;
  name: string;
  businessType: string;
  description: string;
}

export interface UpdateTaxBusinessEntityFormData {
  name: string;
  businessType: string;
  description: string;
}
