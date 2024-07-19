// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.Implementations;
using Liminal.Storage.Abstractions;
using Liminal.Storage.Models;

public class DefaultAttachmentFactory<TAttachment> : IAttachmentFactory<TAttachment>
    where TAttachment : FileAttachment
{
    public TAttachment Create(string externalId, string diskName, string fileName, string mimeType, string extension, long size,
        bool isTransient, Guid? userId)
    {
        var result = Activator.CreateInstance(
            typeof(TAttachment),
            externalId,
            diskName,
            fileName,
            mimeType,
            extension,
            size,
            isTransient,
            userId) ?? throw new Exception("Cannot create attachment");

        if (result is not TAttachment attachment)
        {
            throw new Exception("Cannot cast to TAttachment");
        }

        return attachment;
    }
}
