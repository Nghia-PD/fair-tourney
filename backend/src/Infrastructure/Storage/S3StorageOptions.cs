namespace Infrastructure.Storage;

public class S3StorageOptions
{
    public const string SectionName = "Storage:S3";

    public string ServiceUrl { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string BucketName { get; set; } = string.Empty;
    public bool ForcePathStyle { get; set; }
}