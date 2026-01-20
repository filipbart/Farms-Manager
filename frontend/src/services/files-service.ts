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

  public static async getFile(
    filePath: string,
    fileType?: FileType,
  ): Promise<Blob | null> {
    const token = AxiosWrapper.getAuthTokenFromHeader();
    try {
      const response = await fetch(
        `${ApiUrl.GetFile}?filePath=${encodeURIComponent(
          filePath,
        )}&fileType=${encodeURIComponent(fileType ?? "")}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        },
      );

      if (!response.ok) return null;
      return await response.blob();
    } catch {
      return null;
    }
  }
}
