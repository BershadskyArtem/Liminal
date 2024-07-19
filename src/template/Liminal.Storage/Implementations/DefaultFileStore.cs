// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage.Implementations;
using Liminal.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public class DefaultFileStore(IServiceProvider services): IFileStore
{
    public IDisk Disk(string diskName) => services.GetRequiredKeyedService<IDisk>(diskName);

    public IDisk this[string diskName] => this.Disk(diskName);
}
