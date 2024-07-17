using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace Liminal.Storage.S3;

public class S3DiskOptions
{
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
    public string Host { get; set; }
    public string Region { get; set; }

    public BasicAWSCredentials ToCredentials() => new(AccessKey, SecretKey);

    public AmazonS3Config ToS3Config() 
        => new()
        {
            ServiceURL = Host,
            ForcePathStyle = true,
            RegionEndpoint = RegionEndpoint.GetBySystemName(Region)
        };

    public AmazonS3Client ToS3Client() => new(ToCredentials(), ToS3Config());
}