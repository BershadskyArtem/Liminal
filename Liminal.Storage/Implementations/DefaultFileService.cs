using Liminal.Storage.Abstractions;
using Liminal.Storage.Models;
using Liminal.Storage.Results;

namespace Liminal.Storage.Implementations;

public class DefaultFileService<TAttachment>(
    IFileAttachmentStore<TAttachment> db,
    IAttachmentFactory<TAttachment> factory,
    IFileStore fileStore) : IFileService<TAttachment>
    where TAttachment : FileAttachment
{
    public async Task<UploadResult<TAttachment>> UploadStreamAsync(
        Stream stream,
        bool isTransient, 
        string mimeType, 
        string fileName,
        string diskName,
        string extension,
        long sizeInBytes,
        Guid? userId)
    {
        string externalId = Guid.NewGuid().ToString();
        
        var disk = fileStore[diskName];

        var attachment = factory.Create(
            externalId, 
            diskName,
            fileName, 
            mimeType, 
            extension,
            sizeInBytes,
            isTransient,
            userId);

        var saved = await db.AddAsync(attachment, true);

        if (!saved)
        {
            return UploadResult<TAttachment>.Failure("Failed to save attachment entry in db");
        }

        await disk.UploadAsync(stream, externalId, mimeType);
        
        return UploadResult<TAttachment>.Success(attachment);
    }
}