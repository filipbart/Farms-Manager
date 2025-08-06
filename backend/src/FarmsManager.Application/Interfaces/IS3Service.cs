using FarmsManager.Application.Common;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Services;

namespace FarmsManager.Application.Interfaces;

public interface IS3Service : IService
{
    Task<PaginationModel<FileModel>> GetFilesByType(FileType fileType);
    Task<FileModel> GetFileAsync(FileType fileType, string path);
    Task<FileModel> GetFileByKeyAsync(string key);
    Task<FileModel> GetFolderAsZipAsync(string folderKeyPrefix);
    Task<bool> FileExistsAsync(FileType fileType, string path);
    Task MoveFileAsync(FileType fileType, string sourcePath, string destinationPath);
    Task<string> UploadFileAsync(byte[] fileBytes, FileType fileType, string path, bool publicRead = false);

    Task<FileDirectoryModel> UploadFileToDirectoryAsync(byte[] fileBytes, FileType fileType,
        string directory, string fileName, bool publicRead = false);

    Task DeleteFileAsync(FileType fileType, string path);
    Task DeleteFolderAsync(FileType fileType, string folderPath);
    string GeneratePreSignedUrl(FileType fileType, string path, string fileName = null);
    Task<bool> FolderExistsAsync(FileType fileType, string prefix);
}