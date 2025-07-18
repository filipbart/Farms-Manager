using FarmsManager.Application.Common;
using FarmsManager.Application.FileSystem;

namespace FarmsManager.Application.Interfaces;

public interface IS3Service : IService
{
    Task<string> UploadFileAsync(byte[] fileBytes, FileType fileType, string path, bool publicRead = false);
    Task DeleteFileAsync(FileType fileType, string path);
    string GeneratePreSignedUrl(FileType fileType, string path, string fileName = null);
}