export default interface InsertionListModel {
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
