using Liminal.Storage.Models;
using Liminal.Storage.Results;

namespace Liminal.Storage.Abstractions;

public interface IFileService<TAttachment> 
    where TAttachment : FileAttachment
{
    public Task<UploadResult<TAttachment>> UploadStreamAsync(
        Stream stream, 
        bool isTransient, 
        string mimeType, 
        string fileName, 
        string diskName, 
        string extension,
        long sizeInBytes,
        Guid? userId);
}