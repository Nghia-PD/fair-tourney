using System.Net;
using System.Security.Cryptography;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage;

public class S3FileStorage : IFileStorage
{
    private readonly IAmazonS3 _s3;
    private readonly S3StorageOptions _options;

    public S3FileStorage(IAmazonS3 s3, IOptions<S3StorageOptions> options)
    {
        _s3 = s3;
        _options = options.Value;
    }

    public async Task<WriteOnceUploadResult> UploadIfNotExistsAsync(string key, Stream content, string contentType, CancellationToken cancellationToken)
    {
        // IFormFile streams are seekable (ASP.NET buffers large uploads to disk), so hash and rewind without copying.
        // Fall back to an in-memory copy only for non-seekable streams.
        await using var buffer = content.CanSeek ? null : new MemoryStream();
        if (buffer is not null)
        {
            await content.CopyToAsync(buffer, cancellationToken);
            content = buffer;
        }

        content.Position = 0;
        var md5 = await MD5.HashDataAsync(content, cancellationToken);
        content.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = content,
            // The caller owns the stream.
            AutoCloseStream = false,
            ContentType = contentType,
            MD5Digest = Convert.ToBase64String(md5),
            // S3 conditional write: rejected with 412 if the key already exists, atomically.
            IfNoneMatch = "*"
        };

        try
        {
            await _s3.PutObjectAsync(request, cancellationToken);
            return WriteOnceUploadResult.Created;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode is HttpStatusCode.PreconditionFailed or HttpStatusCode.Conflict)
        {
            var existing = await _s3.GetObjectMetadataAsync(_options.BucketName, key, cancellationToken);

            // For a single-part PUT the ETag is the hex MD5 of the content.
            var sameContent = string.Equals(existing.ETag.Trim('"'), Convert.ToHexStringLower(md5), StringComparison.OrdinalIgnoreCase);

            return sameContent
                ? WriteOnceUploadResult.AlreadyExistsWithSameContent
                : WriteOnceUploadResult.AlreadyExistsWithDifferentContent;
        }
    }
}
