import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface HatcheryRowModel extends AuditFields {
  id: string;
  name: string;
  producerNumber: string;
  fullName: string;
  nip: string;
  address: string;
  dateCreatedUtc: Date;
}
