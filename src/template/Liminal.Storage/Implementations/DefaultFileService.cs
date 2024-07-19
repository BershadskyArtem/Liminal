// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.Implementations;
using Liminal.Storage.Abstractions;
using Liminal.Storage.Models;
using Liminal.Storage.Results;

public class DefaultFileService<TAttachment>(
    IFileAttachmentStore<TAttachment> db,
    IAttachmentFactory<TAttachment> factory,
    IFileStore fileStore): IFileService<TAttachment>
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
        var externalId = Guid.NewGuid().ToString();

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
