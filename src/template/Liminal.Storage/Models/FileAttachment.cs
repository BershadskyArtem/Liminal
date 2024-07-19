// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.Models;
using Liminal.Common.Domain.Models;

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
        if (isTransient == this.IsTransient)
        {
            return;
        }

        this.IsTransient = isTransient;

        if (this.IsTransient)
        {
            this.BecameNotTransientAt = DateTimeOffset.UtcNow;
        }
    }
}
