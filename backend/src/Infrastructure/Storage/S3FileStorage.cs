using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces;
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

    public async Task UploadAsync(string key, Stream content, string contentType, CancellationToken
cancellationToken)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType
        };

        await _s3.PutObjectAsync(request, cancellationToken);
    }
}
