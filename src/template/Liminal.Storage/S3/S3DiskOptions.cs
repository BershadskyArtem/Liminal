// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.S3;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;

public class S3DiskOptions
{
    public string AccessKey { get; set; }

    public string SecretKey { get; set; }

    public string BucketName { get; set; }

    public string Host { get; set; }

    public string Region { get; set; }

    public BasicAWSCredentials ToCredentials() => new(this.AccessKey, this.SecretKey);

    public AmazonS3Config ToS3Config()
        => new()
        {
            ServiceURL = this.Host,
            ForcePathStyle = true,
            RegionEndpoint = RegionEndpoint.GetBySystemName(this.Region),
        };

    public AmazonS3Client ToS3Client()
    {
        if (this._client is not null)
        {
            return this._client;
        }

        var credentials = new BasicAWSCredentials(this.AccessKey, this.SecretKey);
        var awsConfig = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(this.Region),
            ServiceURL = this.Host,
            ForcePathStyle = true,
        };

        this._client = new AmazonS3Client(credentials, awsConfig);

        return this._client;
    }

    private AmazonS3Client? _client;
}
