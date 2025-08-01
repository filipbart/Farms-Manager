export interface HatcheryNote {
  id: string;
  title: string;
  content: string;
}

export interface GetNotesResponse {
  items: HatcheryNote[];
}

export interface HatcheryNoteData {
  title: string;
  content: string;
}
