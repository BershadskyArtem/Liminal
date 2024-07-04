using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Liminal.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Storage.S3;

public class DefaultS3Disk : IDisk
{
    public string Name { get; }

    public DefaultS3Disk(
        string name,
        IServiceProvider serviceProvider)
    {
        Name = name;
        var diskOptions = serviceProvider.GetRequiredKeyedService<S3DiskOptions>(Name);
        _client = diskOptions.ToS3Client();
    }
    
     public async Task UploadAsync(Stream stream, string key, string contentType)
    {
        var request = new PutObjectRequest()
        {
            BucketName = Name,
            Key = key,
            InputStream = stream,
            ContentType = contentType
        };

        var uploadResponse = await _client.PutObjectAsync(request);

        if (uploadResponse.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new ArgumentException($"Status: {uploadResponse.HttpStatusCode}, key: {key}, req: {uploadResponse.ResponseMetadata.RequestId}");
        }
    }

    public async Task DeleteByIdAsync(string id)
    {
        var request = new DeleteObjectRequest()
        {
            Key = id,
            BucketName = Name
        };


        var uploadResponse = await _client.DeleteObjectAsync(request);

        if (uploadResponse.HttpStatusCode != HttpStatusCode.NoContent)
        {
            throw new ArgumentException($"Status: {uploadResponse.HttpStatusCode}, key: {id}, req: {uploadResponse.ResponseMetadata.RequestId}");
        }
    }

    public async Task<Stream> GetFileByIdAsync(string id)
    {
        var request = new GetObjectRequest()
        {
            BucketName = Name,
            Key = id,
        };

        using (var objectResponse = await _client.GetObjectAsync(request))
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

    public Task<string> GetPublicLinkByIdAsync(string id)
    {
        // TODO: Make this thing work for other providers too.
        return Task.FromResult($"https://{Name}.s3.amazonaws.com/id");
    }

    public async Task<string> GetPublicSignedLinkByIdAsync(string id, TimeSpan validFor)
    {
        var request = new GetPreSignedUrlRequest()
        {
            Expires = DateTime.UtcNow.Add(validFor),
            Key = id,
            BucketName = Name
        };

        var presignedUrlResponse = await _client.GetPreSignedURLAsync(request);

        if (presignedUrlResponse is null)
        {
            throw new ArgumentException($"Cannot get url for media with key:");
        }

        return presignedUrlResponse;
    }

    private readonly AmazonS3Client _client;
}