using Liminal.Storage.Abstractions;
using Liminal.Storage.Models;

namespace Liminal.Storage.Implementations;

public class DefaultAttachmentFactory<TAttachment> : IAttachmentFactory<TAttachment> 
    where TAttachment : FileAttachment
{
    public TAttachment Create(string externalId, string diskName, string fileName, string mimeType, string extension, long size,
        bool isTransient, Guid? userId)
    {
        var result = Activator.CreateInstance(
            typeof(TAttachment),
            externalId,
            diskName,
            fileName,
            mimeType,
            extension,
            size,
            isTransient,
            userId);

        if (result is null)
        {
            throw new Exception("Cannot create attachment");
        }

        if (result is not TAttachment attachment)
        {
            throw new Exception("Cannot cast to TAttachment");
        }

        return attachment;
    }
}