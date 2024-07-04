using Liminal.Common.Domain.Models;

namespace Liminal.Storage.Models;

public class FileAttachment : AuditableEntity
{
    public Guid? UserId { get; set; }
    public string ExternalId { get; set; }
    public bool IsTransient { get; protected set; }
    public DateTimeOffset? BecameNotTransientAt { get; protected set; }
    public long Size { get; set; }
    public string Extension { get; set; }
    public string FileName { get; set; }
    public string? MimeType { get; set; }
    public string DiskName { get; set; }

    public void SetTransient(bool isTransient)
    {
        if (isTransient == IsTransient)
        {
            return;
        }
        
        IsTransient = isTransient;

        if (IsTransient)
        {
            BecameNotTransientAt = DateTimeOffset.UtcNow;    
        }
    }

    public FileAttachment(string externalId, 
        string diskName, 
        string fileName, 
        string mimeType,
        string extension, 
        long size,
        bool isTransient,
        Guid? userId)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Id = Guid.NewGuid();
        ExternalId = externalId;
        DiskName = diskName;
        MimeType = mimeType;
        Extension = extension;
        FileName = fileName;
        Size = size;
        IsTransient = isTransient;
        UserId = userId;
    }
}