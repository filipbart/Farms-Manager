import type { AuditFields } from "../../common/interfaces/audit-fields";

export interface ProductionDataTransferFeedListModel extends AuditFields {
  id: string;
  fromCycleId: string;
  fromCycleText: string;
  fromFarmId: string;
  fromFarmName: string;
  fromHenhouseName: string;
  toCycleId: string;
  toCycleText: string;
  toFarmId: string;
  toFarmName: string;
  toHenhouseName: string;
  feedName: string;
  tonnage: number;
  value: number;
  dateCreatedUtc: Date;
}

export interface AddTransferFeedData {
  fromFarmId: string;
  fromHenhouseId: string;
  fromCycleId: string;
  toFarmId: string;
  toHenhouseId: string;
  toCycleId: string;
  feedName: string;
  tonnage: number;
  value: number;
}

export interface UpdateTransferFeedData {
  fromCycleId: string;
  toCycleId: string;
  tonnage: number;
  value: number;
}
