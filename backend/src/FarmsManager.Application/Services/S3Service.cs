using System.Net;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using FarmsManager.Application.Common;
using FarmsManager.Application.FileSystem;
using FarmsManager.Application.Interfaces;
using FarmsManager.Domain.Exceptions;
using FarmsManager.Shared.Extensions;
using Microsoft.Extensions.Configuration;

namespace FarmsManager.Application.Services;

public class S3Service : IS3Service
{
    private const double ExpiresInMinutes = 20;
    private readonly string _bucketName;
    private readonly IAmazonS3 _s3Client;

    private readonly bool _isDevelopment =
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    private bool _bucketExists;

    public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration.GetValue<string>("S3:BucketName");
    }

    private async Task EnsureBucketExistsAsync()
    {
        if (_bucketExists)
            return;

        var request = new ListBucketsRequest();
        var response = await _s3Client.ListBucketsAsync(request);

        if (response.Buckets?.All(t => t.BucketName != _bucketName) ?? true)
        {
            await _s3Client.PutBucketAsync(new PutBucketRequest
            {
                BucketName = _bucketName,
                UseClientRegion = true,
                ObjectLockEnabledForBucket = false
            });

            var createBucketRequest = new PutBucketVersioningRequest
            {
                BucketName = _bucketName,
                VersioningConfig = new S3BucketVersioningConfig
                {
                    Status = VersionStatus.Enabled,
                    EnableMfaDelete = false
                },
                ChecksumAlgorithm = ChecksumAlgorithm.SHA256
            };

            await _s3Client.PutBucketVersioningAsync(createBucketRequest);
        }

        _bucketExists = true;
    }

    public async Task<bool> FileExistsAsync(FileType fileType, string path)
    {
        await EnsureBucketExistsAsync();
        var key = GetPath(fileType, path);

        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectMetadataAsync(request);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            throw new Exception($"Błąd podczas sprawdzania istnienia pliku w S3: {ex.Message}", ex);
        }
    }

    public async Task<PaginationModel<FileModel>> GetFilesByType(FileType fileType)
    {
        await EnsureBucketExistsAsync();

        var prefix = fileType + "/";

        var request = new ListObjectsRequest
        {
            BucketName = _bucketName,
            Prefix = prefix
        };
        var response = await _s3Client.ListObjectsAsync(request);

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            return new PaginationModel<FileModel>
            {
                TotalRows = 0,
                Items = new List<FileModel>()
            };
        }

        var objects = response.S3Objects.Select((t, index) => new FileModel
        {
            Id = index,
            IsFile = true,
            IsDirectory = false,
            LastModifyDate = t.LastModified,
            FileName = t.Key.Replace(prefix, "")
        }).ToList();

        return new PaginationModel<FileModel>
        {
            TotalRows = objects.Count,
            Items = objects
        };
    }


    public async Task<FileModel> GetFileAsync(FileType fileType, string path)
    {
        await EnsureBucketExistsAsync();

        var key = GetPath(fileType, path);

        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        var obj = await _s3Client.GetObjectAsync(request);
        if (obj.HttpStatusCode != HttpStatusCode.OK)
        {
            throw DomainException.FileNotFound();
        }

        using var ms = new MemoryStream();
        await obj.ResponseStream.CopyToAsync(ms);

        return new FileModel
        {
            IsFile = true,
            IsDirectory = false,
            LastModifyDate = obj.LastModified,
            ContentType = obj.Headers.ContentType,
            FileName = Path.GetFileName(path),
            Data = ms.ToArray()
        };
    }

    public async Task MoveFileAsync(FileType fileType, string sourcePath, string destinationPath)
    {
        await EnsureBucketExistsAsync();

        var sourceKey = GetPath(fileType, sourcePath);
        var destinationKey = GetPath(fileType, destinationPath);

        var copyRequest = new CopyObjectRequest
        {
            SourceBucket = _bucketName,
            SourceKey = sourceKey,
            DestinationBucket = _bucketName,
            DestinationKey = destinationKey
        };

        try
        {
            var copyResponse = await _s3Client.CopyObjectAsync(copyRequest);
            if (copyResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Błąd podczas kopiowania pliku w S3.");
            }

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = sourceKey
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }
        catch (AmazonS3Exception ex)
        {
            throw new Exception($"Błąd podczas przenoszenia pliku w S3: {ex.Message}", ex);
        }
    }


    public async Task<string> UploadFileAsync(byte[] fileBytes, FileType fileType, string path, bool publicRead = false)
    {
        await EnsureBucketExistsAsync();

        using var ms = new MemoryStream(fileBytes);
        var key = GetPath(fileType, path);

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            CannedACL = publicRead ? S3CannedACL.PublicRead : S3CannedACL.Private,
            InputStream = ms
        };

        try
        {
            var s3Response = await _s3Client.PutObjectAsync(request);
            if (s3Response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Błąd podczas zapisywania pliku do S3");
            }
        }
        catch (AmazonS3Exception e)
        {
            throw new Exception($"Błąd podczas próby zapisania pliku do S3: {e.Message}");
        }

        return key;
    }

    public async Task DeleteFileAsync(FileType fileType, string path)
    {
        await EnsureBucketExistsAsync();

        var key = GetPath(fileType, path);
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(request);
    }

    public string GeneratePreSignedUrl(FileType fileType, string path, string fileName = null)
    {
        var key = GetPath(fileType, path);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Expires = DateTime.Now.AddMinutes(ExpiresInMinutes),
            Protocol = Protocol.HTTP,
        };

        if (fileName.IsNotEmpty())
        {
            if (_isDevelopment)
                return _s3Client.GetPreSignedURL(request);

            var contentDisposition = $"attachment; filename={fileName}";

            request.ResponseHeaderOverrides = new ResponseHeaderOverrides
            {
                ContentDisposition = contentDisposition
            };
        }

        return _s3Client.GetPreSignedURL(request);
    }


    private static string GetPath(FileType fileType, string path)
    {
        if (path.Contains(fileType + "/"))
            return path;

        var pathBuilder = new StringBuilder();
        pathBuilder.Append(fileType.ToString());
        pathBuilder.Append('/');
        pathBuilder.Append(path);
        return pathBuilder.ToString();
    }
}