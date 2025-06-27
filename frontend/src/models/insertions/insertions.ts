export default interface InsertionListModel {
  id: string;
  cycleText: string;
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
}
