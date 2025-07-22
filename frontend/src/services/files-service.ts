import ApiUrl from "../common/ApiUrl";
import type { PaginateModel } from "../common/interfaces/paginate";
import type { FileModel } from "../models/files/file-model";
import type { FileType } from "../models/files/file-type";
import AxiosWrapper from "../utils/axios/wrapper";

export class FilesService {
  public static async getFilesByType(fileType: FileType) {
    return await AxiosWrapper.get<PaginateModel<FileModel>>(ApiUrl.Files, {
      fileType,
    });
  }
}
