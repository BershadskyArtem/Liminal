using Liminal.Storage.Models;

namespace Liminal.Storage.Abstractions;

public interface IFileAttachmentStore<TAttachment>
    where TAttachment : FileAttachment
{
    public Task<bool> AddAsync(TAttachment attachment, bool save = false);
    public Task<List<TAttachment>> GetExpiredAttachmentsAsync(DateTimeOffset allBefore, int batchSize = 5);
    public Task<TAttachment?> GetByIdAsync(Guid id);
    public Task<TAttachment?> GetByExternalIdAsync(string externalId);
    public Task<bool> DeleteById(Guid id, bool save = false);
    public Task<bool> DeleteByExternalId(string id, bool save = false);
    public Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}