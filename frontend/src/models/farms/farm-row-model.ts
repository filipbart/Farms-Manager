import type { HouseRowModel } from "./house-row-model";
import type CycleDto from "./latest-cycle";

export default interface FarmRowModel {
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
