import type { HouseRowModel } from "./house-row-model";
import type CycleDto from "./latest-cycle";
import type { AuditFields } from "../../common/interfaces/audit-fields";

export default interface FarmRowModel extends AuditFields {
  id: string;
  name: string;
  producerNumber: string;
  nip: string;
  address: string;
  henHousesCount: number;
  dateCreatedUtc: Date;
  henhouses: HouseRowModel[];
  activeCycle: CycleDto;
}
