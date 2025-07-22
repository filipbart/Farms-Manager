using FarmsManager.Application.Common;
using FarmsManager.Application.FileSystem;

namespace FarmsManager.Application.Interfaces;

public interface IS3Service : IService
{
    Task<PaginationModel<FileModel>> GetFilesByType(FileType fileType);
    Task<FileModel> GetFileAsync(FileType fileType, string path);
    Task<bool> FileExistsAsync(FileType fileType, string path);
    Task MoveFileAsync(FileType fileType, string sourcePath, string destinationPath);
    Task<string> UploadFileAsync(byte[] fileBytes, FileType fileType, string path, bool publicRead = false);
    Task DeleteFileAsync(FileType fileType, string path);
    string GeneratePreSignedUrl(FileType fileType, string path, string fileName = null);
}