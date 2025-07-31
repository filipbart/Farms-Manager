export interface GasContractorRow {
  id: string;
  name: string;
  nip: string;
  address: string;
  dateCreatedUtc: string;
}

export interface GasContractorsQueryResponse {
  contractors: GasContractorRow[];
}
