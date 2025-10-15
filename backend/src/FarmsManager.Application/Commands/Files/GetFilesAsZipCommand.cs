using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using MediatR;
using System.IO.Compression;

namespace FarmsManager.Application.Commands.Files;

public record GetFilesAsZipDto(
    List<string> FilePaths,
    FileType? FileType);

public record GetFilesAsZipCommand(List<string> FilePaths, FileType? FileType) : IRequest<byte[]>;

public class GetFilesAsZipCommandHandler : IRequestHandler<GetFilesAsZipCommand, byte[]>
{
    private readonly IS3Service _s3Service;

    public GetFilesAsZipCommandHandler(IS3Service s3Service)
    {
        _s3Service = s3Service;
    }

    public async Task<byte[]> Handle(GetFilesAsZipCommand request, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var filePath in request.FilePaths)
            {
                var decodedPath = Uri.UnescapeDataString(filePath);
                var file = !request.FileType.HasValue
                    ? await _s3Service.GetFileByKeyAsync(decodedPath)
                    : await _s3Service.GetFileAsync(request.FileType.Value, decodedPath);

                if (file is { HasData: true })
                {
                    var zipEntry = archive.CreateEntry(file.FileName, CompressionLevel.Optimal);
                    await using var zipEntryStream = zipEntry.Open();
                    await zipEntryStream.WriteAsync(file.Data.AsMemory(0, file.Data.Length), cancellationToken);
                }
            }
        }

        return memoryStream.ToArray();
    }
}
