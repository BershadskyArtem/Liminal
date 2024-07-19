// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.S3;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Liminal.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public class DefaultS3Disk : IDisk
{
    public string Name { get; }

    public DefaultS3Disk(
        string name,
        IServiceProvider serviceProvider)
    {
        this.Name = name;
        this._options = serviceProvider.GetRequiredKeyedService<S3DiskOptions>(this.Name);
        this._client = this._options.ToS3Client();
    }

    public async Task UploadAsync(Stream stream, string key, string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName = this._options.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
        };

        var uploadResponse = await this._client.PutObjectAsync(request);

        if (uploadResponse.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new ArgumentException($"Status: {uploadResponse.HttpStatusCode}, key: {key}, req: {uploadResponse.ResponseMetadata.RequestId}");
        }
    }

    public async Task DeleteByIdAsync(string id)
    {
        var request = new DeleteObjectRequest
        {
            Key = id,
            BucketName = this._options.BucketName,
        };

        var uploadResponse = await this._client.DeleteObjectAsync(request);

        if (uploadResponse.HttpStatusCode != HttpStatusCode.NoContent)
        {
            throw new ArgumentException($"Status: {uploadResponse.HttpStatusCode}, key: {id}, req: {uploadResponse.ResponseMetadata.RequestId}");
        }
    }

    public async Task<Stream> GetFileByIdAsync(string id)
    {
        var request = new GetObjectRequest
        {
            BucketName = this._options.BucketName,
            Key = id,
        };

        using (var objectResponse = await this._client.GetObjectAsync(request))
        {
            using (var responseStream = objectResponse.ResponseStream)
            {
                var stream = new MemoryStream();
                await responseStream.CopyToAsync(stream);
                stream.Position = 0;
                return stream;
            }
        }
    }

    public Task<string> GetPublicLinkByIdAsync(string id) =>

        // TODO: Make this thing work for other providers too.
        Task.FromResult($"https://{this.Name}.s3.amazonaws.com/id");

    public async Task<string> GetPublicSignedLinkByIdAsync(string id, TimeSpan validFor)
    {
        var request = new GetPreSignedUrlRequest
        {
            Expires = DateTime.UtcNow.Add(validFor),
            Key = id,
            BucketName = this.Name,
        };

        var presignedUrlResponse = await this._client.GetPreSignedURLAsync(request) ?? throw new ArgumentException($"Cannot get url for media with key:");

        return presignedUrlResponse;
    }

    private readonly AmazonS3Client _client;
    private readonly S3DiskOptions _options;
}
