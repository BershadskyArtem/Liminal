// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.EntityFrameworkCore.Abstractions;
using Liminal.Storage.Models;
using Microsoft.EntityFrameworkCore;

public interface IAttachmentDbContext<TAttachment>
    where TAttachment : FileAttachment
{
    public DbSet<TAttachment> Set();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
