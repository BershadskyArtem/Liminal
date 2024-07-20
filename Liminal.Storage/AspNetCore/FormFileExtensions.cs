using Microsoft.AspNetCore.Http;

namespace Liminal.Storage.AspNetCore;

public static class FormFileExtensions
{
    public static FormFileInfo ToFileInfo(this IFormFile file) => new(file);
}