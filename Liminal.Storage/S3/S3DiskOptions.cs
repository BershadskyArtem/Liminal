using Amazon.Runtime;
using Amazon.S3;

namespace Liminal.Storage.S3;

public class S3DiskOptions
{
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
    public string Host { get; set; }

    public BasicAWSCredentials ToCredentials()
        => new BasicAWSCredentials(AccessKey, SecretKey);

    public AmazonS3Config ToS3Config()
        => new AmazonS3Config()
        {
            ServiceURL = Host,
            ForcePathStyle = true
        };

    public AmazonS3Client ToS3Client()
        => new AmazonS3Client(ToCredentials(), ToS3Config());
}