import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface HouseRowModel extends AuditFields {
  id: string;
  name: string;
  code: string;
  area: number;
  description: string;
  farmId: string;
  dateCreatedUtc: Date;
}
