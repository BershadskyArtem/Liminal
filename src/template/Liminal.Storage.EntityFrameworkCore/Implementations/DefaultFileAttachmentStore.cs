// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.EntityFrameworkCore.Implementations;
using Liminal.Storage.Abstractions;
using Liminal.Storage.EntityFrameworkCore.Abstractions;
using Liminal.Storage.Models;
using Microsoft.EntityFrameworkCore;

public class DefaultFileAttachmentStore<TAttachment>(IAttachmentDbContext<TAttachment> context): IFileAttachmentStore<TAttachment>
    where TAttachment : FileAttachment
{
    protected DbSet<TAttachment> Data => context.Set();

    public async Task<bool> AddAsync(TAttachment attachment, bool save = false)
    {
        await this.Data.AddAsync(attachment);
        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<List<TAttachment>> GetExpiredAttachmentsAsync(DateTimeOffset allBefore, int batchSize = 5) => await this.Data
            .Where(f => f.CreatedAt < allBefore && f.IsTransient)
            .Take(batchSize)
            .ToListAsync();

    public async Task<TAttachment?> GetByIdAsync(Guid id) => await this.Data.FirstOrDefaultAsync(f => f.Id == id);

    public async Task<TAttachment?> GetByExternalIdAsync(string externalId) => await this.Data.FirstOrDefaultAsync(f => f.ExternalId == externalId);

    public async Task<bool> DeleteById(Guid id, bool save = false)
    {
        var attachment = await this.GetByIdAsync(id);

        if (attachment is null)
        {
            return true;
        }

        this.Data.Remove(attachment);

        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<bool> DeleteByExternalId(string id, bool save = false)
    {
        var attachment = await this.GetByExternalIdAsync(id);

        if (attachment is null)
        {
            return true;
        }

        this.Data.Remove(attachment);

        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken) > 0;

    private async Task<bool> MaybeSaveChangesAsync(bool save, CancellationToken cancellationToken = default)
    {
        if (!save)
        {
            return true;
        }

        return await this.SaveChangesAsync(cancellationToken);
    }
}
