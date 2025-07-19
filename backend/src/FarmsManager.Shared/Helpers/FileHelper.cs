using Microsoft.AspNetCore.StaticFiles;

namespace FarmsManager.Shared.Helpers;

public static class FileHelper
{
    public static string GetFileContentType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();
        provider.TryGetContentType(fileName, out var contentType);
        return contentType;
    }
}