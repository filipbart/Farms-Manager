using FarmsManager.Application.Common;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Interfaces;

public interface IS3Service : IService
{
    Task<string> UploadFileAsync(IFormFile file, string key);
    string GeneratePreSignedUrl(string key);
}