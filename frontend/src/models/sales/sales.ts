export default interface SalesListModel {
  id: string;
  cycleText: string;
  farmName: string;
  henhouseName: string;
  type: string;
  typeDesc: string;
  saleDate: Date;
  //todo uzupełnić

  dateCreatedUtc: Date;
  internalGroupId: string;
  dateIrzSentUtc?: Date;
  isSentToIrz: boolean;
  documentNumber?: string;
}
