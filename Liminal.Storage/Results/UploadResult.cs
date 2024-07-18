using Liminal.Storage.Models;

namespace Liminal.Storage.Results;

public class UploadResult<TAttachment>
    where TAttachment : FileAttachment
{
    public TAttachment Entry { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    
    
    public static UploadResult<TAttachment> Failure(string errorMessage)
    {
        return new UploadResult<TAttachment>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }

    public static UploadResult<TAttachment> Success(TAttachment entry)
    {
        return new UploadResult<TAttachment>
        {
            IsSuccess = true,
            Entry = entry
        };
    }
}