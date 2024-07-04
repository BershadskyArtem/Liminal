using Liminal.Storage.Models;

namespace Liminal.Storage.Abstractions;

public interface IAttachmentFactory<out TAttachment>
    where TAttachment : FileAttachment
{
    public TAttachment Create(
        string externalId, 
        string diskName, 
        string fileName, 
        string mimeType,
        string extension, 
        long size,
        bool isTransient,
        Guid? userId);
}