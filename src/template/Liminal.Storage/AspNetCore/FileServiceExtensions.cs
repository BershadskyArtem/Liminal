// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.AspNetCore;
using Liminal.Storage.Abstractions;
using Liminal.Storage.Models;
using Liminal.Storage.Results;
using Microsoft.AspNetCore.Http;

public static class FileServiceExtensions
{
    public static async Task<UploadResult<TAttachment>> UploadFormFileAsync<TAttachment>(
        this IFileService<TAttachment> service,
        IFormFile file,
        bool isTransient,
        string diskName,
        Guid? authorId)
        where TAttachment : FileAttachment
    {
        var stream = file.OpenReadStream();

        try
        {
            var mimeType = file.ContentType;
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            var size = file.Length;

            var result = await service.UploadStreamAsync(
                stream,
                isTransient,
                mimeType,
                fileName,
                diskName,
                extension,
                size,
                authorId);

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await stream.DisposeAsync();
        }
    }
}
