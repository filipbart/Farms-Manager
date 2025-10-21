import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface SlaughterhouseRowModel extends AuditFields {
  id: string;
  name: string;
  producerNumber: string;
  nip: string;
  address: string;
  dateCreatedUtc: Date;
}
