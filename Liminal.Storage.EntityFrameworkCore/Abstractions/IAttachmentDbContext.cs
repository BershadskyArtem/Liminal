using Liminal.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Storage.EntityFrameworkCore.Abstractions;

public interface IAttachmentDbContext<TAttachment> 
    where TAttachment : FileAttachment
{
    public DbSet<TAttachment> Set();
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}