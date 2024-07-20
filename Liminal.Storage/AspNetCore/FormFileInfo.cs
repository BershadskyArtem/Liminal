using Microsoft.AspNetCore.Http;

namespace Liminal.Storage.AspNetCore;

public class FormFileInfo(IFormFile file)
{
    public string FileName { get; set; } = Path.GetFileNameWithoutExtension(file.FileName);
    public string Extension { get; set; } = Path.GetExtension(file.FileName);
    public string MimeType { get; set; } = file.ContentType;
    public long Size { get; set; } = file.Length;

    public Stream ToStream()
    {
        return file.OpenReadStream();
    }
}