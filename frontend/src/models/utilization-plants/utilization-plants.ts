export interface UtilizationPlantRowModel {
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
