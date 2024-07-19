// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.Abstractions;
using Liminal.Storage.Models;

public interface IAttachmentFactory<out TAttachment>
    where TAttachment : FileAttachment
{
    public TAttachment Create(
        string externalId,
        string diskName,
        string fileName,
        string mimeType,
        string extension,
        long size,
        bool isTransient,
        Guid? userId);
}
