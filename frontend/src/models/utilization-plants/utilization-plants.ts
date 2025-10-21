import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface UtilizationPlantRowModel extends AuditFields {
  id: string;
  name: string;
  irzNumber: string;
  nip: string;
  address: string;
  dateCreatedUtc: string;
}

export interface AddUtilizationPlantFormData {
  name: string;
  irzNumber: string;
  nip: string;
  address: string;
}
