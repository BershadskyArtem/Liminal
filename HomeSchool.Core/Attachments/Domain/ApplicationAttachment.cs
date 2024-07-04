using Liminal.Storage.Models;

namespace HomeSchool.Core.Attachments.Domain;

public class ApplicationAttachment : FileAttachment
{
    public ApplicationAttachment(
        string externalId,
        string diskName,
        string fileName,
        string mimeType,
        string extension,
        long size,
        bool isTransient,
        Guid? userId) :
        base(
            externalId,
            diskName,
            fileName,
            mimeType,
            extension,
            size,
            isTransient,
            userId)
    { }
}