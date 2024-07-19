// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.Abstractions;
using Liminal.Storage.Models;
using Liminal.Storage.Results;

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
