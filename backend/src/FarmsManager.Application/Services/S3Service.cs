using Amazon.S3.Model;
using FarmsManager.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FarmsManager.Application.Services;

public class S3Service : IS3Service
{
    const decimal ExpiresInMinutes = 10;
    public Task<string> UploadFileAsync(IFormFile file, string key)
    {
        throw new NotImplementedException();
    }

    public string GeneratePreSignedUrl(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            
        }
    }
}