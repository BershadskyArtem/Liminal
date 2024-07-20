using Liminal.Storage.Abstractions;
using Liminal.Storage.EntityFrameworkCore.Abstractions;
using Liminal.Storage.EntityFrameworkCore.Implementations;
using Liminal.Storage.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Storage.EntityFrameworkCore;

public static class LiminalStorageBuilderExtensions
{
    public static LiminalStorageBuilder UseEntityFrameworkStores<TDbContext, TAttachment>(this LiminalStorageBuilder builder)
        where TDbContext : class, IAttachmentDbContext<TAttachment>
        where TAttachment : FileAttachment
    {
        builder.Services.AddScoped<IAttachmentDbContext<TAttachment>, TDbContext>();
        builder.Services.AddScoped<IFileAttachmentStore<TAttachment>, DefaultFileAttachmentStore<TAttachment>>();
        
        return builder; 
    }
}