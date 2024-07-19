// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.EntityFrameworkCore;
using Liminal.Storage.Abstractions;
using Liminal.Storage.EntityFrameworkCore.Abstractions;
using Liminal.Storage.EntityFrameworkCore.Implementations;
using Liminal.Storage.Models;
using Microsoft.Extensions.DependencyInjection;

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
