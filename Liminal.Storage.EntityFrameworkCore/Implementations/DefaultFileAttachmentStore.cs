using Liminal.Storage.Abstractions;
using Liminal.Storage.EntityFrameworkCore.Abstractions;
using Liminal.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Storage.EntityFrameworkCore.Implementations;

public class DefaultFileAttachmentStore<TAttachment>(IAttachmentDbContext<TAttachment> context) : IFileAttachmentStore<TAttachment> 
    where TAttachment : FileAttachment
{
    protected DbSet<TAttachment> Data => context.Set();
    
    public async Task<bool> AddAsync(TAttachment attachment, bool save = false)
    {
        await Data.AddAsync(attachment);
        return await MaybeSaveChangesAsync(save);
    }

    public async Task<List<TAttachment>> GetExpiredAttachmentsAsync(DateTimeOffset allBefore, int batchSize)
    {
        return await Data
            .Where(f => f.CreatedAt < allBefore && f.IsTransient)
            .Take(batchSize)
            .ToListAsync();
    }

    public async Task<TAttachment?> GetByIdAsync(Guid id)
    {
        return await Data.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<TAttachment?> GetByExternalIdAsync(string externalId)
    {
        return await Data.FirstOrDefaultAsync(f => f.ExternalId == externalId);
    }

    public async Task<bool> DeleteById(Guid id, bool save = false)
    {
        var attachment = await GetByIdAsync(id);

        if (attachment is null)
        {
            return true;
        }
        
        Data.Remove(attachment);

        return await MaybeSaveChangesAsync(save);
    }

    public async Task<bool> DeleteByExternalId(string id, bool save = false)
    {
        var attachment = await GetByExternalIdAsync(id);

        if (attachment is null)
        {
            return true;
        }
        
        Data.Remove(attachment);

        return await MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
    
    private async Task<bool> MaybeSaveChangesAsync(bool save, CancellationToken cancellationToken = default)
    {
        if (!save)
            return true;

        return await SaveChangesAsync(cancellationToken);
    }
}