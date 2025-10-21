import type { AuditFields } from "../../common/interfaces/audit-fields";

export default interface InsertionListModel extends AuditFields {
  id: string;
  cycleId: string;
  cycleText: string;
  farmId: string;
  farmName: string;
  henhouseName: string;
  insertionDate: Date;
  quantity: number;
  hatcheryName: string;
  bodyWeight: number;
  dateCreatedUtc: Date;
  internalGroupId: string;
  dateIrzSentUtc?: Date;
  isSentToIrz: boolean;
  documentNumber?: string;
  irzComment?: string;
  reportedToWios: boolean;
  wiosComment?: string;
}
