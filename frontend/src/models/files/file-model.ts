export interface FileModel {
  fileName: string;
  isFile: boolean;
  isDirectory: boolean;
  creationDate: string;
  lastModifyDate?: string;
  contentType: string;
  data?: Uint8Array;
  hasData: boolean;
}
